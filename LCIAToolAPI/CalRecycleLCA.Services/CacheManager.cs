using System;
using System.IO;
using System.Configuration;
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

        private void LogTo(string logfile, string msg)
        {
            using (StreamWriter appendLog = File.AppendText(logfile))
                appendLog.WriteLine(msg);
        }

        private string toc(DateTime start)
        {
            return (DateTime.Now - start).ToString(@"mm\:ss");
        }

        public List<int> InitializeCache()
        {
            var logPath = ConfigurationManager.AppSettings["LogPath"];
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            string logFile = Path.Combine(logPath, "ConfigInit.log");

            LogTo(logFile, "");
            DateTime start = DateTime.Now;
            LogTo(logFile, string.Format("{0}: Beginning /config/init",start.ToString("s")));
            List<int> frags = _FragmentService.Queryable().Select(k => k.FragmentID).ToList();
            List<Scenario> scenarios = _ScenarioService.Queryable()
                .Where(k => k.ScenarioID != Scenario.MODEL_BASE_CASE_ID)
                .ToList();

            int i = 1;

            LogTo(logFile, string.Format("Computing {0} Fragments over {1} Scenarios", frags.Count(), 
                scenarios.Count()));
            
            // first- compute FragmentFlowLCIA for all fragments for base scenario
            foreach (int frag in frags)
            {
                _FragmentLCIAComputation.FragmentLCIAComputeSave(frag, Scenario.MODEL_BASE_CASE_ID);
                LogTo(logFile,string.Format("[{0}] Completed base case traversal for FragmentID {1} of {2}", toc(start),frag, frags.Count()));
            }

            foreach (Scenario s in scenarios)
            {
                _FragmentLCIAComputation.FragmentLCIAComputeSave(s.TopLevelFragmentID, s.ScenarioID);
                LogTo(logFile, string.Format("[{0}] Completed scenario data for ScenarioID {1} ({2} of {3})", toc(start), s.ScenarioID, ++i, scenarios.Count()));
            }
            LogTo(logFile,string.Format( "{0}: /config/init finished.",DateTime.Now.ToString("s")));
            return new List<int>() { frags.Count, scenarios.Count };
        }
 
        private void CloneRefScenario(int fragmentId, int newScenarioId, int refScenarioId)
        {
            var sw = new CounterTimer();
            sw.CStart();
            sw.Click("zero");
            var fs = _FragmentLCIAComputation.FragmentsEncountered(fragmentId, refScenarioId);

            sw.Click("traced");

            var nc = _NodeCacheService.Queryable()
                .Where(k => k.ScenarioID == refScenarioId)
                .Where(k => fs.Contains(k.FragmentFlow.FragmentID)).ToList()
                .Select(k => new NodeCache
                {
                    ILCDEntityID = k.ILCDEntityID,
                    ScenarioID = newScenarioId,
                    FragmentFlowID = k.FragmentFlowID,
                    NodeWeight = k.NodeWeight,
                    FlowMagnitude = k.FlowMagnitude,
                    ObjectState = ObjectState.Added
                });
            
            sw.Click(String.Format("{0} nodeCaches created",nc.Count()));

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

            sw.Click(String.Format("{0} scoreCaches created", sc.Count()));

            if (refScenarioId != Scenario.MODEL_BASE_CASE_ID)
                _ScenarioService.CloneScenarioElements(newScenarioId, refScenarioId);

            sw.Click("elements cloned");
            
            _unitOfWork.SetAutoDetectChanges(false);
            _NodeCacheService.InsertGraphRange(nc);
            _ScoreCacheService.InsertGraphRange(sc);
            sw.Click("inserts");
            _unitOfWork.SaveChanges();
            sw.Click("saved");
            _unitOfWork.SetAutoDetectChanges(true);
            sw.Stop();
            return;
        }

        public ScenarioGroupResource CreateScenarioGroup(ScenarioGroupResource postdata)
        {

            ScenarioGroup newGroup = _ScenarioGroupService.Query(k => k.Secret == postdata.Secret)
                .Select()
                .FirstOrDefault();

            if (newGroup == null)
            {
                newGroup = _ScenarioGroupService.AddAuthenticatedScenarioGroup(postdata);
                _unitOfWork.SaveChanges();
            }
            return _ScenarioGroupService.GetResource(newGroup.ScenarioGroupID);
        }

        public ScenarioGroupResource UpdateScenarioGroup(int scenarioGroupId, ScenarioGroupResource putdata)
        {
            ScenarioGroup currentGroup = _ScenarioGroupService.UpdateScenarioGroup(scenarioGroupId, putdata);
            _unitOfWork.SaveChanges();
            return _ScenarioGroupService.GetResource(currentGroup.ScenarioGroupID);
        }


        public ScenarioResource PublishScenario(int scenarioId, int targetGroup = ScenarioGroup.BASE_SCENARIO_GROUP) // this needs _unitOfWork
        {
            // no error checking here, no thinking. just do it.
            if (_ScenarioService.PublishScenario(scenarioId, targetGroup) == null)
                return new ScenarioResource();
            else
            {
                _unitOfWork.SaveChanges();
                return _ScenarioService.GetResource(scenarioId);
            }
        }


        public int CreateScenario(ScenarioResource postScenario, int refScenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            var sw = new CounterTimer();
            sw.CStart();
            Scenario newScenario = _ScenarioService.NewScenario(postScenario);

            sw.Click("created");
            
            _unitOfWork.SaveChanges(); // sets flag

            sw.Click("saved");

            CloneRefScenario(postScenario.TopLevelFragmentID, newScenario.ScenarioID, refScenarioId);

            sw.Click("cloned");

            _FragmentLCIAComputation.FragmentLCIAComputeSave(newScenario.TopLevelFragmentID, newScenario.ScenarioID); // clears flag
            sw.Click("computed");
            sw.Stop();
            return newScenario.ScenarioID;
        }

        public void DeleteScenario(int scenarioId)
        {
            _ScenarioService.DeleteScenario(scenarioId);
            _unitOfWork.SaveChanges();
        }

        public bool ImplementScenarioChanges(int scenarioId, CacheTracker cacheTracker)
        {
            // NodeCache contains traversal results- needs to be reset for fragment and its parents when a fragment traversal is changed
            // ScoreCache contains LCIA computations- for processes, these do not depend on traversal; for subfragments, they do
            // they are also LCIA-method-specific.
            // so ways of clearing NodeCache include: 
            //  * for entire scenario 
            //  * for a list of fragments in a scenario (&)
            // ways of clearing ScoreCache include:
            //  * for an entire scenario
            //  * for a single LCIA method across a scenario
            //  * for a list of individual fragment flows in a scenario
            //  * for all fragment flows of type 2 (subfragments) in a scenario
            //  * for all fragment flows of type 2 within a set of fragments in a scenario (&)
            // Any time either cache is modified, subfragment totals must be cleared in all parent fragments.
            //  = Node: Entire Scenario --> clear all subfragment nodes
            //  = Node: List of Fragments --> clear subfragment nodes in ParentFragments(traversal)
            //  = Score: entire scenario [null]
            //  = Score: LCIA Method - will clear subfragment nodes automatically
            //  = Score: individual FFs --> clear subfragment nodes in ParentFragments(FFs)
            //  = Score: subfragment nodes - will clear subfragment nodes intrinsically
            //  = Score: subfragment nodes in fragment list - will do the rest
            // The two routes marked (&) use the ParentFragments() method to generate a list of fragments that are ancestors of a set of fragmentFlows.

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
                _ScoreCacheService.ClearScoreCacheForFragments(scenarioId);
                _NodeCacheService.ClearNodeCacheByScenario(scenarioId);
            }
            if (cacheTracker.ScoreCacheStale)
                _ScoreCacheService.ClearScoreCacheByScenario(scenarioId);

            List<int> frags = new List<int>();
            if (cacheTracker.FragmentFlowsTraverse.Count() > 0)
            {
                frags.AddRange(_FragmentLCIAComputation.ParentFragments(cacheTracker.FragmentFlowsTraverse, scenarioId));
                _NodeCacheService.ClearNodeCacheByScenarioAndFragments(frags, scenarioId);
            }

            sw.Click("node");
            if (cacheTracker.FragmentFlowsStale.Count() > 0)
            {
                frags.AddRange(_FragmentLCIAComputation.ParentFragments(cacheTracker.FragmentFlowsStale, scenarioId));
                _ScoreCacheService.ClearScoreCacheByScenario(scenarioId, cacheTracker.FragmentFlowsStale);
            }

            if (frags.Count() > 0)
                _ScoreCacheService.ClearScoreCacheForParentFragments(frags, scenarioId);

            sw.Click("score");
            if (_ScenarioService.IsStale(scenarioId))
            {
                // IsStale indicates another thread is performing cache update-- so our changes are invalid
                //_unitOfWork.Rollback(); // this doesn't work since we haven't defined a transaction- just abandon work
                _unitOfWork.SetAutoDetectChanges(true);
                return false;
            }

            _ScenarioService.MarkStale(scenarioId);

            _unitOfWork.SaveChanges(); // update database with changes
            _unitOfWork.SetAutoDetectChanges(true);
            sw.Click("save");
            if (cacheTracker.Recompute)
            {
                _FragmentLCIAComputation.FragmentLCIAComputeSave(scenarioId);
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
