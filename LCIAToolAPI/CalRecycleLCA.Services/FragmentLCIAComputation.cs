﻿using Entities.Models;
using LcaDataModel;
using Ninject;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public class FragmentLCIAComputation : IFragmentLCIAComputation
    {
        [Inject]
        private readonly IFragmentTraversalV2 _fragmentTraversalV2;
        [Inject]
        private readonly ILCIAComputationV2 _lciaComputationV2;
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly INodeCacheService _nodeCacheService;
        [Inject]
        private readonly IScoreCacheService _scoreCacheService;
        [Inject]
        private readonly ILCIAMethodService _lciaMethodService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;


        // diagnostics
        private CounterTimer sw_local, sw_ff, sw_cache, sw_lcia, sw_traverse;
        private List<ScoreCache> currentCache;


        public FragmentLCIAComputation(IFragmentTraversalV2 fragmentTraversalV2,
            ILCIAComputationV2 lciaComputationV2,
            IFragmentFlowService fragmentFlowService,
            IScoreCacheService scoreCacheService,
            INodeCacheService nodeCacheService,
            ILCIAMethodService lciaMethodService,
            IUnitOfWork unitOfWork)
        {
            if (fragmentTraversalV2 == null)
            {
                throw new ArgumentNullException("fragmentTraversalV2 is null");
            }
            _fragmentTraversalV2 = fragmentTraversalV2;

            if (lciaComputationV2 == null)
            {
                throw new ArgumentNullException("lciaComputationV2 is null");
            }
            _lciaComputationV2 = lciaComputationV2;

            if (fragmentFlowService == null)
            {
                throw new ArgumentNullException("fragmentFlowService is null");
            }
            _fragmentFlowService = fragmentFlowService;

            if (scoreCacheService == null)
            {
                throw new ArgumentNullException("scoreCacheService is null");
            }
            _scoreCacheService = scoreCacheService;

            if (nodeCacheService == null)
            {
                throw new ArgumentNullException("nodeCacheService is null");
            }
            _nodeCacheService = nodeCacheService;

            if (lciaMethodService == null)
            {
                throw new ArgumentNullException("lciaMethodService is null");
            }
            _lciaMethodService = lciaMethodService;

            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork is null");
            }
            _unitOfWork = unitOfWork;

            sw_local = new CounterTimer();
            sw_ff = new CounterTimer();
            sw_cache = new CounterTimer();
            sw_lcia = new CounterTimer();
            sw_traverse = new CounterTimer();

            currentCache = new List<ScoreCache>();
        }

        public IEnumerable<NodeCache> FragmentTraverse(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // this will eventually become private after diagnostics are done
            if (_nodeCacheService.IsCached(fragmentId, scenarioId))
                return _nodeCacheService.Queryable()
                    //.Where(nc => nc.FragmentFlow.FragmentID == fragmentId)
                    .Where(nc => nc.ScenarioID == scenarioId).ToList();
            else
            {
                var nodeCaches = _fragmentTraversalV2.Traverse((int)fragmentId, scenarioId).Select(k => new NodeCache
                {
                    FragmentFlowID = k.FragmentFlowID,
                    ScenarioID = k.ScenarioID,
                    FlowMagnitude = k.FlowMagnitude,
                    NodeWeight = k.NodeWeight,
                    ObjectState = ObjectState.Added
                });
                _unitOfWork.SetAutoDetectChanges(false);
                _nodeCacheService.InsertGraphRange(nodeCaches);
                _unitOfWork.SaveChanges();
                _unitOfWork.SetAutoDetectChanges(true);
                
                return nodeCaches;
            }


        }




        public IEnumerable<ScoreCache> FragmentLCIAComputeNoSave(int fragmentId, int scenarioId)
        {
            sw_local.CStart();

            FragmentTraverse(fragmentId, scenarioId);
            sw_local.Click("traversal");
            
            var lciaMethods = _lciaMethodService.QueryActiveMethods();
            sw_local.Click("lcia");

            var newCaches = FragmentFlowLCIA(fragmentId, scenarioId, lciaMethods);//.ToList();

            sw_local.CStop();
            sw_ff.CStop();
            sw_cache.CStop();
            sw_lcia.CStop();

            return newCaches;
            


        }

        public void FragmentLCIAComputeSave(int fragmentId, int scenarioId)
        {
            sw_local.CStart();

            FragmentTraverse(fragmentId, scenarioId);
            sw_local.Click("traversal");

            var lciaMethods = _lciaMethodService.QueryActiveMethods();
            sw_local.Click("lcia");

            FragmentFlowLCIA(fragmentId, scenarioId, lciaMethods, true);//.ToList();

            sw_local.CStop();
            sw_ff.CStop();
            sw_cache.CStop();
            sw_lcia.CStop();

            return;

        }

        public IEnumerable<FragmentLCIAModel> FragmentLCIA(int? fragmentId, int? scenarioId, int? lciaMethodId)
        {
            // for now: return one record per FragmentFlow
            var lcia = _fragmentFlowService.Queryable()
                .Where(x => x.FragmentID == fragmentId)
                 .Join(_nodeCacheService.Queryable().Where(x => x.ScenarioID == scenarioId)
      , ff => ff.FragmentFlowID
      , nc => nc.FragmentFlowID
      , (ff, nc) => new { ff, nc })
                .Join(_scoreCacheService.Queryable().Where(x => x.ScenarioID == scenarioId && x.LCIAMethodID == lciaMethodId)
      , l => l.nc.FragmentFlowID
      , sc => sc.FragmentFlowID
      , (nc, sc) => new { nodeCaches = nc, scoreCaches = sc }).Select(s => new FragmentLCIAModel
            {
                FragmentFlowID = s.nodeCaches.ff.FragmentFlowID,
                FragmentStageID = s.nodeCaches.ff.FragmentStageID,
                NodeWeight = s.nodeCaches.nc.NodeWeight,
                ImpactScore = s.scoreCaches == null ? 0 : s.scoreCaches.ImpactScore,
                Result = s.nodeCaches.nc.NodeWeight * (s.scoreCaches == null ? 0 : s.scoreCaches.ImpactScore)
                //NodeLCIAResults = new List<LCIAModel>()
            });

            return lcia;
        }

        private IEnumerable<ScoreCache> FragmentFlowLCIA(int fragmentId, int scenarioId, IEnumerable<int> lciaMethods,
            bool save = false)
        {
            // set score cache for fragment / scenario / method: iterate through
            // fragmentflows 
            currentCache.AddRange(_scoreCacheService.GetScenarioCaches(fragmentId, scenarioId).ToList());

            sw_local.Click("caches");

            List<ScoreCache> scoreCaches = new List<ScoreCache>();

            var fragmentFlows = _fragmentFlowService.GetLCIAFlows(fragmentId);

            sw_local.Click("GTF");


            foreach (var item in fragmentFlows)
            {
                
                sw_ff.CStart();
                //if (item.FlowID == null)
                  //  item.FlowID = 0; // TODO: this should be set to whatever is the fragment's inflow-- ??

                // case 1: terminate FF directly -- judged FASTER 346-425 ms
                var fragmentNode = _fragmentFlowService.Terminate(item, scenarioId, true); // true => do Background 
                // case 2: terminate via GetResource -- judged SLOWER 533-582 ms
                //var fragmentNode = _fragmentFlowService.Terminate(_fragmentFlowService.GetResource(item), scenarioId, true);
                // propagate to Traversal


                sw_ff.CStop();
                
                if (fragmentNode.NodeTypeID == 2)
                {
                    //recursive LCIA computation, results to cache
                    sw_local.Click("recurse " + Convert.ToString((int)fragmentNode.SubFragmentID));
                    FragmentFlowLCIA((int)fragmentNode.SubFragmentID, scenarioId, lciaMethods, save);
                }

                scoreCaches.AddRange(GetScoreCaches(item.FragmentFlowID, fragmentNode, lciaMethods));

            }

            if (scoreCaches.Count() > 0 && save == true)
            {
                sw_cache.CStart();
                //disable this until results have been cached - doesn't seem to increase performance
                //_unitOfWork.SetAutoDetectChanges(false);
                _scoreCacheService.InsertGraphRange(scoreCaches);

                _unitOfWork.SaveChanges();
                //enable this after results have been cached -  it was turned off to increase performance
                //_unitOfWork.SetAutoDetectChanges(true);
                sw_cache.CStop();
            }

            return scoreCaches; 
        }



        //   public void SetScoreCache(int? targetId, int? nodeTypeId, IEnumerable<LCIAMethod> lciaMethods, int scenarioId, int fragmentFlowId)
        // {
        //   error('not implemented');
        //}
        public IEnumerable<ScoreCache> GetScoreCaches(int fragmentFlowId, FlowTerminationModel fragmentNode, IEnumerable<int> lciaMethods)
        {
            List<ScoreCache> scoreCachesInProgress = new List<ScoreCache>();

            sw_lcia.CStart();

            IEnumerable<int> haveLciaMethods = currentCache.Where(x => x.FragmentFlowID == fragmentFlowId)
                                                        .Select(x => x.LCIAMethodID);

            IEnumerable<int> needLciaMethods = lciaMethods.Where(m => !haveLciaMethods.Contains(m));

            if (needLciaMethods.Count() == 0)
                return scoreCachesInProgress;

            switch (fragmentNode.NodeTypeID)
            {
                case 1:

                    var scores = _lciaComputationV2.ProcessLCIA(fragmentNode.ProcessID, needLciaMethods, fragmentNode.ScenarioID);

                    foreach (var lciaMethodId in needLciaMethods.AsQueryable())
                    {
                        if (scores.Any(s => s.LCIAMethodID == lciaMethodId))
                        {
                            double score = scores
                                .Where(x => x.LCIAMethodID == lciaMethodId)
                                .Select(x => Convert.ToDouble(x.Total)).First();

                            scoreCachesInProgress.Add(new ScoreCache()
                            {
                                ScenarioID = fragmentNode.ScenarioID,
                                FragmentFlowID = fragmentFlowId,
                                LCIAMethodID = lciaMethodId,
                                ImpactScore = score,
                            });

                        }
                    }

                    foreach (var processScoreCache in scoreCachesInProgress)
                    {
                        processScoreCache.ObjectState = ObjectState.Added;
                    }

                    break;
                case 2:
                    foreach (var lciaMethodId in needLciaMethods)
                    {
                        var lcias = FragmentLCIA(fragmentNode.SubFragmentID, fragmentNode.ScenarioID, lciaMethodId);

                        scoreCachesInProgress.Add(new ScoreCache()
                        {
                            ScenarioID = fragmentNode.ScenarioID,
                            FragmentFlowID = fragmentFlowId,
                            LCIAMethodID = lciaMethodId,
                            ImpactScore = Convert.ToDouble(lcias.Sum(a => a.Result))
                        });
                    }

                    foreach (var fragmentScoreCache in scoreCachesInProgress)
                    {
                        fragmentScoreCache.ObjectState = ObjectState.Added;
                    }
                    break;
            }  /* end of switch NodeType */

            sw_lcia.CStop();

            return scoreCachesInProgress;
        } /* end of SetScoreCache */
    }
}
