﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using Ninject;
using LcaDataModel;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public class CacheManager : ICacheManager
    {
        [Inject]
        private readonly IScenarioService _ScenarioService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;
        [Inject]
        private readonly IFragmentService _FragmentService;
        [Inject]
        private readonly IFragmentLCIAComputation _FragmentLCIAComputation;
        [Inject]
        private readonly INodeCacheService _NodeCacheService;
        [Inject]
        private readonly IScoreCacheService _ScoreCacheService;
        [Inject]
        private readonly IScenarioGroupService _ScenarioGroupService;


        private T verifiedDependency<T>(T dependency) where T : class
        {
            if (dependency == null)
            {
                throw new ArgumentNullException("dependency", String.Format("Type: {0}", dependency.GetType().ToString()));
            }
            else
            {
                return dependency;
            }
        }

        public CacheManager(IScenarioService scenarioService,
            IUnitOfWork unitOfWork,
            IFragmentService fragmentService,
            IFragmentLCIAComputation fragmentLCIAComputation,
            INodeCacheService nodeCacheService,
            IScoreCacheService scoreCacheService,
            IScenarioGroupService scenarioGroupService)
        {
            _ScenarioService = verifiedDependency(scenarioService);
            _unitOfWork = verifiedDependency(unitOfWork);
            _FragmentService = verifiedDependency(fragmentService);
            _FragmentLCIAComputation = verifiedDependency(fragmentLCIAComputation);
            _NodeCacheService = verifiedDependency(nodeCacheService);
            _ScoreCacheService = verifiedDependency(scoreCacheService);
            _ScenarioGroupService = verifiedDependency(scenarioGroupService);
        }

        public void InitializeCache()
        {
            // first- compute FragmentFlowLCIA for all fragments for base scenario
            List<int> frags = _FragmentService.Queryable().Select(k => k.FragmentID).ToList();
            foreach (int frag in frags)
                _FragmentLCIAComputation.FragmentLCIAComputeSave(frag, Scenario.MODEL_BASE_CASE_ID);

            List<Scenario> scenarios = _ScenarioService.Queryable()
                .Where(k => k.ScenarioID != Scenario.MODEL_BASE_CASE_ID)
                .ToList();

            foreach (Scenario s in scenarios)
                _FragmentLCIAComputation.FragmentLCIAComputeSave(s.TopLevelFragmentID, s.ScenarioID);
        }
 
        private void CloneRefScenario(int fragmentId, int newScenarioId, int refScenarioId)
        {
            var fs = _FragmentLCIAComputation.FragmentsEncountered(fragmentId, refScenarioId);

            var nc = _NodeCacheService.Queryable()
                .Where(k => k.ScenarioID == refScenarioId)
                .Where(k => fs.Contains(k.FragmentFlow.FragmentID)).ToList()
                .Select(k => new NodeCache
                {
                    ScenarioID = newScenarioId,
                    FragmentFlowID = k.FragmentFlowID,
                    NodeWeight = k.NodeWeight,
                    FlowMagnitude = k.FlowMagnitude,
                    ObjectState = ObjectState.Added
                });
            
            var sc = _ScoreCacheService.Queryable()
                .Where(k => k.ScenarioID == refScenarioId)
                .Where(k => fs.Contains(k.FragmentFlow.FragmentID)).ToList()
                .Select(k => new ScoreCache
                {
                    ScenarioID = newScenarioId,
                    FragmentFlowID = k.FragmentFlowID,
                    LCIAMethodID = k.LCIAMethodID,
                    ImpactScore = k.ImpactScore,
                    ObjectState = ObjectState.Added
                });

            if (refScenarioId != Scenario.MODEL_BASE_CASE_ID)
                _ScenarioService.CloneScenarioElements(newScenarioId, refScenarioId);

            _unitOfWork.SetAutoDetectChanges(false);
            _NodeCacheService.InsertGraphRange(nc);
            _ScoreCacheService.InsertGraphRange(sc);
            _unitOfWork.SaveChanges();
            _unitOfWork.SetAutoDetectChanges(true);
        }

        public ScenarioGroupResource CreateScenarioGroup(ScenarioGroupResource postdata)
        {
            ScenarioGroup newGroup = _ScenarioGroupService.AddAuthenticatedScenarioGroup(postdata);
            _unitOfWork.SaveChanges();
            return _ScenarioGroupService.GetResource(newGroup.ScenarioGroupID);
        }

        public int CreateScenario(ScenarioResource postScenario, int refScenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            Scenario newScenario = _ScenarioService.NewScenario(postScenario);
            _unitOfWork.SaveChanges(); // sets flag

            CloneRefScenario(postScenario.TopLevelFragmentID, newScenario.ScenarioID, refScenarioId);

            _FragmentLCIAComputation.FragmentLCIAComputeSave(newScenario.TopLevelFragmentID, newScenario.ScenarioID); // clears flag
            return newScenario.ScenarioID;
        }

        public void DeleteScenario(int scenarioId)
        {
            _ScenarioService.DeleteScenario(scenarioId);
            _unitOfWork.SaveChanges();
        }

        public bool ImplementScenarioChanges(int scenarioId, CacheTracker cacheTracker)
        {
            var sw = new CounterTimer();
            sw.CStart();
            // first, we do the most surgical operations: clear LCIA method-specific results
            _unitOfWork.SetAutoDetectChanges(false);
            if (cacheTracker.LCIAMethodsStale != null)
            {
                cacheTracker.Recompute = true; // set this explicitly
                foreach (int method in cacheTracker.LCIAMethodsStale)
                {
                    _ScoreCacheService.ClearScoreCacheByScenarioAndLCIAMethod(scenarioId, method);
                }
            }
            sw.Click("methods");
            if (cacheTracker.NodeCacheStale)
            {
                // next, clear all computed fragment scores
                _ScoreCacheService.ClearScoreCacheForSubFragments(scenarioId);
                _NodeCacheService.ClearNodeCacheByScenario(scenarioId);
            }
            sw.Click("node");
            if (cacheTracker.ScoreCacheStale)
                _ScoreCacheService.ClearScoreCacheByScenario(scenarioId);
            else
                if (cacheTracker.FragmentFlowsStale.Count() > 0)
                {
                    _ScoreCacheService.ClearScoreCacheForSubFragments(scenarioId);
                    _ScoreCacheService.ClearScoreCacheByScenario(scenarioId,cacheTracker.FragmentFlowsStale);
                }

            sw.Click("score");
            if (_ScenarioService.IsStale(scenarioId))
            {
                // IsStale indicates another thread is performing cache update-- so our changes are invalid
                _unitOfWork.Rollback();
                _unitOfWork.SetAutoDetectChanges(true);
                return false;
            }

            _ScenarioService.MarkStale(scenarioId);

            _unitOfWork.SaveChanges(); // update database with changes
            _unitOfWork.SetAutoDetectChanges(true);
            sw.Click("save");
            if (cacheTracker.Recompute)
            {
                int tlf = _ScenarioService.Query(k => k.ScenarioID == scenarioId).Select(k => k.TopLevelFragmentID).First();
                _FragmentLCIAComputation.FragmentLCIAComputeSave(tlf, scenarioId);
            }
            sw.Click("recompute");
            sw.CStop();
            return true;
        }

        /// <summary>
        /// Delete NodeCache data by ScenarioId
        /// </summary>
        public void ClearNodeCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            CacheTracker cacheTracker = new CacheTracker();
            cacheTracker.NodeCacheStale = true;
            ImplementScenarioChanges(scenarioId, cacheTracker);
        }

        /*
        /// <summary>
        /// Delete NodeCache data by ScenarioID and FragmentID
        /// </summary>
        public void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _NodeCacheService.ClearNodeCacheByScenarioAndFragment(scenarioId, fragmentId);
            _unitOfWork.SaveChanges();
        }
         * */

        /// <summary>
        /// Delete ScoreCache data by ScenarioId
        /// </summary>
        public void ClearScoreCacheByScenario(int scenarioId)
        {
            //_unitOfWork.SetAutoDetectChanges(false); 
            CacheTracker cacheTracker = new CacheTracker();
            cacheTracker.ScoreCacheStale = true;
            ImplementScenarioChanges(scenarioId, cacheTracker);
            //_unitOfWork.SetAutoDetectChanges(true);
        }

        /*
        /// <summary>
        /// Delete ScoreCache data by ScenarioID and FragmentID
        /// </summary>
        public void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _ScoreCacheService.ClearScoreCacheByScenarioAndFragment(scenarioId, fragmentId);
            _unitOfWork.SaveChanges();
        }
        */
        /// <summary>
        /// Delete ScoreCache data by ScenarioID and LCIAMethodID
        /// </summary>
        public void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId, int lciaMethodId)
        {
            CacheTracker cacheTracker = new CacheTracker();
            cacheTracker.LCIAMethodsStale.Add(lciaMethodId);
            ImplementScenarioChanges(scenarioId, cacheTracker);
        }

    }
}
