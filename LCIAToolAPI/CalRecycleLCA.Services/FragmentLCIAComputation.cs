using Entities.Models;
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
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly INodeCacheService _nodeCacheService;
        [Inject]
        private readonly IService<ScoreCache> _scoreCacheService;
        //[Inject]
        //private readonly IFragmentNodeProcessService _fragmentNodeProcessService;
        //[Inject]
        //private readonly IProcessSubstitutionService _processSubstitutionService;
        //[Inject]
        //private readonly IFragmentNodeFragmentService _fragmentNodeFragmentService;
        //[Inject]
        //private readonly IFragmentSubstitutionService _fragmentSubstitutionService;
        [Inject]
        private readonly ILCIAMethodService _lciaMethodService;
        // [Inject]
        // private readonly IScenarioBackgroundService _scenarioBackgroundService;
        [Inject]
        private readonly IBackgroundService _backgroundService;
        [Inject]
        private readonly IProcessFlowService _processFlowService;
        [Inject]
        private readonly IProcessEmissionParamService _processEmissionParamService;
        [Inject]
        private readonly IFlowService _flowService;
        [Inject]
        private readonly IFlowFlowPropertyService _flowFlowPropertyService;
        [Inject]
        private readonly IFlowPropertyParamService _flowPropertyParamService;
        [Inject]
        private readonly IFlowPropertyEmissionService _flowPropertyEmissionService;
        [Inject]
        private readonly IProcessDissipationService _processDissipationService;
        [Inject]
        private readonly IProcessDissipationParamService _processDissipationParamService;
        [Inject]
        private readonly ILCIAService _lciaService;
        [Inject]
        private readonly ICharacterizationParamService _characterizationParamService;
        [Inject]
        private readonly IParamService _paramService;
        [Inject]
        private readonly IDependencyParamService _dependencyParamService;
        [Inject]
        private readonly IFragmentService _fragmentService;
        [Inject]
        private readonly IProcessService _processService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;

        public FragmentLCIAComputation(IFragmentFlowService fragmentFlowService,
            IScoreCacheService scoreCacheService,
            INodeCacheService nodeCacheService,
            //IFragmentNodeProcessService fragmentNodeProcessService,
            //IProcessSubstitutionService processSubstitutionService,
            //IFragmentNodeFragmentService fragmentNodeFragmentService,
            //IFragmentSubstitutionService fragmentSubstitutionService,
            ILCIAMethodService lciaMethodService,
            // IScenarioBackgroundService scenarioBackgroundService,
            IBackgroundService backgroundService,
            IProcessFlowService processFlowService,
            IProcessEmissionParamService processEmissionParamService,
            IFlowService flowService,
            IFlowFlowPropertyService flowFlowPropertyService,
            IFlowPropertyParamService flowPropertyParamService,
            IFlowPropertyEmissionService flowPropertyEmissionService,
            IProcessDissipationService processDissipationService,
            IProcessDissipationParamService processDissipationParamService,
            ILCIAService lciaService,
            ICharacterizationParamService characterizationParamService,
            IParamService paramService,
            IDependencyParamService dependencyParamService,
            IFragmentService fragmentService,
            IProcessService processService,
            IUnitOfWork unitOfWork)
        {
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
            /*
            if (fragmentNodeProcessService == null)
            {
                throw new ArgumentNullException("fragmentNodeProcessService is null");
            }
            _fragmentNodeProcessService = fragmentNodeProcessService;

            if (processSubstitutionService == null)
            {
                throw new ArgumentNullException("processSubstitutionService is null");
            }
            _processSubstitutionService = processSubstitutionService;

            if (fragmentNodeFragmentService == null)
            {
                throw new ArgumentNullException("fragmentNodeFragmentService is null");
            }
            _fragmentNodeFragmentService = fragmentNodeFragmentService;

            if (fragmentSubstitutionService == null)
            {
                throw new ArgumentNullException("fragmentSubstitutionService is null");
            }
            _fragmentSubstitutionService = fragmentSubstitutionService;
            */
            // if (scenarioBackgroundService == null)
            // {
            //     throw new ArgumentNullException("scenarioBackgroundService is null");
            // }
            // _scenarioBackgroundService = scenarioBackgroundService;


            if (backgroundService == null)
            {
                throw new ArgumentNullException("backgroundService is null");
            }
            _backgroundService = backgroundService;

            if (processFlowService == null)
            {
                throw new ArgumentNullException("processFlowService is null");
            }
            _processFlowService = processFlowService;

            if (processEmissionParamService == null)
            {
                throw new ArgumentNullException("processEmissionParamService is null");
            }
            _processEmissionParamService = processEmissionParamService;

            if (flowService == null)
            {
                throw new ArgumentNullException("flowService is null");
            }
            _flowService = flowService;

            if (flowFlowPropertyService == null)
            {
                throw new ArgumentNullException("flowFlowPropertyService is null");
            }
            _flowFlowPropertyService = flowFlowPropertyService;

            if (flowPropertyParamService == null)
            {
                throw new ArgumentNullException("flowPropertyParamService is null");
            }
            _flowPropertyParamService = flowPropertyParamService;

            if (flowPropertyEmissionService == null)
            {
                throw new ArgumentNullException("flowPropertyEmissionService is null");
            }
            _flowPropertyEmissionService = flowPropertyEmissionService;

            if (processDissipationService == null)
            {
                throw new ArgumentNullException("processDissipationService is null");
            }
            _processDissipationService = processDissipationService;

            if (processDissipationParamService == null)
            {
                throw new ArgumentNullException("processDissipationParamService is null");
            }
            _processDissipationParamService = processDissipationParamService;

            if (lciaService == null)
            {
                throw new ArgumentNullException("lciaService is null");
            }
            _lciaService = lciaService;

            if (characterizationParamService == null)
            {
                throw new ArgumentNullException("characterizationParamService is null");
            }
            _characterizationParamService = characterizationParamService;

            if (paramService == null)
            {
                throw new ArgumentNullException("paramService is null");
            }
            _paramService = paramService;

            if (lciaMethodService == null)
            {
                throw new ArgumentNullException("lciaMethodService is null");
            }
            _lciaMethodService = lciaMethodService;

            if (dependencyParamService == null)
            {
                throw new ArgumentNullException("dependencyParamService is null");
            }
            _dependencyParamService = dependencyParamService;

            if (fragmentService == null)
            {
                throw new ArgumentNullException("fragmentService is null");
            }
            _fragmentService = fragmentService;

            if (processService == null)
            {
                throw new ArgumentNullException("processService is null");
            }
            _processService = processService;

            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork is null");
            }
            _unitOfWork = unitOfWork;
        }

        public void FragmentLCIACompute(int fragmentId, int scenarioId)
        {

            var lciaMethods = _lciaMethodService.QueryActiveMethods().ToList();
            FragmentFlowLCIA(fragmentId, scenarioId, lciaMethods);//.ToList();

        }

        public IEnumerable<FragmentLCIAModel> ComputeFragmentLCIA(int? fragmentId, int? scenarioId, int? lciaMethodId)
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
                NodeWeight = s.nodeCaches.nc.NodeWeight,
                ImpactScore = s.scoreCaches == null ? 0 : s.scoreCaches.ImpactScore,
                Result = s.nodeCaches.nc.NodeWeight * (s.scoreCaches == null ? 0 : s.scoreCaches.ImpactScore)
                //NodeLCIAResults = new List<LCIAModel>()
            });

            return lcia;
        }

        public void FragmentFlowLCIA(int? fragmentId, int scenarioId, IEnumerable<LCIAMethod> lciaMethods)
        {
            // set score cache for fragment / scenario / method: iterate through
            // fragmentflows 


            // this does nothing if traversal has already been completed - FIGURE OUT THIS PART ON THURSDAY
            FragmentTraversalV2 fragmentTraversalV2 = new FragmentTraversalV2(//_flowService,
                            _fragmentFlowService,
                            _nodeCacheService,
                //_fragmentNodeProcessService,
                            _processFlowService,
                //_fragmentNodeFragmentService,
                            _flowFlowPropertyService,
                            _dependencyParamService,
                //_flowPropertyParamService,
                //_fragmentService,
                //_paramService,
                            _unitOfWork);

            fragmentTraversalV2.Traverse((int)fragmentId, scenarioId);

            var fragmentFlows = _fragmentFlowService.GetCachedFlows((int)fragmentId, scenarioId);

            foreach (var item in fragmentFlows)
            {
                if (item.FlowID == null)
                    item.FlowID = 0; // TODO: this should be set to whatever is the fragment's inflow-- ??

                var fragmentNode = _fragmentFlowService.Terminate(item, scenarioId, true); // true => do Background 

                if (fragmentNode.NodeTypeID == 2)
                {
                    //recursive LCIA computation, results to cache
                    FragmentFlowLCIA(fragmentNode.SubFragmentID, fragmentNode.ScenarioID, lciaMethods);
                }

                SetScoreCache(item.FragmentFlowID, fragmentNode, lciaMethods);

            }

            return; // fragmentFlows
            /*                  .Select(x => new FragmentLCIAModel
                              {
                                  FragmentFlowID = x.FragmentFlowID,
                                  NodeTypeID = x.NodeTypeID,
                                  FlowID = x.FlowID,
                                  DirectionID = x.DirectionID
                              });
             * */
        }


        /* *************
        public ResolveBackgroundModel ResolveBackground(int? flowId, int? directionId, int? scenarioId, int? nodeTypeId)
        {
            IEnumerable<ResolveBackgroundModel> background = null;
            int targetId;
            background = _scenarioBackgroundService.Queryable()
                .Where(x => x.ScenarioID == scenarioId)
                .Where(x => x.FlowID == flowId)
                .Where(x => x.DirectionID == directionId)
               .Select(bg => new ResolveBackgroundModel
               {
                   NodeTypeID = bg.NodeTypeID,
                   ILCDEntityID = bg.ILCDEntityID
               });

            if (background.Count() == 0)
            {
                background = _backgroundService.Queryable()
                .Where(x => x.FlowID == flowId)
                .Where(x => x.DirectionID == directionId)
               .Select(bg => new ResolveBackgroundModel
               {
                   NodeTypeID = bg.NodeTypeID,
                   ILCDEntityID = bg.ILCDEntityID
               });

                if (background.Count() == 0)
                {
                    throw new ArgumentNullException("background is null");
                }
            }

            nodeTypeId = Convert.ToInt32(background.Select(x => x.NodeTypeID).FirstOrDefault());

            //if (nodeTypeId == 5)
            //{
            //    targetId = 0;
            //}
            //else
            //{
            //    targetId = Convert.ToInt32(background.Select(x => x.TargetID).FirstOrDefault());
            //}


            switch (nodeTypeId)
            {
                case 1:
                    targetId = _processService.Queryable().Where(x => x.ILCDEntityID == background.Select(y => y.ILCDEntityID).FirstOrDefault())
                    .Select(z=> (int)z.ProcessID).FirstOrDefault();
                    break;
                case 2:
                    targetId = _fragmentService.Queryable().Where(x => x.ILCDEntityID == background.Select(y => y.ILCDEntityID).FirstOrDefault())
                    .Select(z => (int)z.FragmentID).FirstOrDefault();
                    break;
                default:
                    targetId = 0;
                        break;
            }


            ResolveBackgroundModel resolveBackground = new ResolveBackgroundModel();
            resolveBackground.NodeTypeID = nodeTypeId;
            resolveBackground.TargetID = targetId;
            return resolveBackground;

        }
        ************* */

        //   public void SetScoreCache(int? targetId, int? nodeTypeId, IEnumerable<LCIAMethod> lciaMethods, int scenarioId, int fragmentFlowId)
        // {
        //   error('not implemented');
        //}
        public void SetScoreCache(int fragmentFlowId, FragmentNodeResource fragmentNode, IEnumerable<LCIAMethod> lciaMethods)
        {
            //disable this until results have been cached - to increase performance
            _unitOfWork.SetAutoDetectChanges(false);

            List<ScoreCache> processScoreCaches = new List<ScoreCache>();
            List<ScoreCache> fragmentScoreCaches = new List<ScoreCache>();

            IEnumerable<int> haveLciaMethods = _scoreCacheService.Queryable()
                                                        .Where(x => x.ScenarioID == fragmentNode.ScenarioID
                                                                && x.FragmentFlowID == fragmentFlowId).AsEnumerable()
                                                        .Select(y => y.LCIAMethodID);

            IEnumerable<LCIAMethod> needLciaMethods = lciaMethods.Where(m => !haveLciaMethods.Contains(m.LCIAMethodID));

            //IEnumerable<int> needLciaMethods = tmpLciaMethods.Except(haveLciaMethods);

            if (needLciaMethods == null)
                return;

            switch (fragmentNode.NodeTypeID)
            {
                case 1:

                    LCIAComputationV2 lciaComputation = new LCIAComputationV2(_processFlowService,
                        //_processEmissionParamService,
                        _lciaMethodService,
                        //_flowService,
                        //_flowFlowPropertyService,
                        //_flowPropertyParamService,
                        //_flowPropertyEmissionService,
                        //_processDissipationService,
                        //_processDissipationParamService,
                        _lciaService);
                    //_characterizationParamService,
                    //_paramService);

                    var scores = lciaComputation.ProcessLCIA(fragmentNode.ProcessID, needLciaMethods, fragmentNode.ScenarioID);

                    foreach (var lciaMethodItem in needLciaMethods.AsQueryable())
                    {
                        double impactScore = 0;
                        if (scores.Any(s => s.LCIAMethodID == lciaMethodItem.LCIAMethodID))
                        {
                            var score = scores
                                .Where(x => x.LCIAMethodID == lciaMethodItem.LCIAMethodID)
                                .Select(x => new ScoreCache
                                    {
                                        ImpactScore = Convert.ToDouble(x.Result)
                                    }).FirstOrDefault();

                            impactScore = Convert.ToDouble(score.ImpactScore);

                            //ScoreCache scoreCache = new ScoreCache();
                            //scoreCache.ScenarioID = fragmentNode.ScenarioID;
                            //scoreCache.FragmentFlowID = fragmentFlowId;
                            //scoreCache.LCIAMethodID = lciaMethodItem.LCIAMethodID;
                            //scoreCache.ImpactScore = impactScore;
                            //scoreCache.ObjectState = ObjectState.Added;
                            //_scoreCacheService.InsertOrUpdateGraph(scoreCache);

                            processScoreCaches.Add(new ScoreCache()
                            {
                                ScenarioID = fragmentNode.ScenarioID,
                                FragmentFlowID = fragmentFlowId,
                                LCIAMethodID = lciaMethodItem.LCIAMethodID,
                                ImpactScore = impactScore
                            });

                        }

                    }

                    foreach (var processScoreCache in processScoreCaches)
                    {
                        processScoreCache.ObjectState = ObjectState.Added;
                    }
                    _scoreCacheService.InsertGraphRange(processScoreCaches);

                    break;
                case 2:
                    foreach (var lciaMethodItem in needLciaMethods)
                    {
                        var lcias = ComputeFragmentLCIA(fragmentNode.SubFragmentID, fragmentNode.ScenarioID, lciaMethodItem.LCIAMethodID);

                        /*
                        if (lcias != null)
                        {
                            var lciaScore = lcias.ToList()
                        .GroupBy(t => new
                        {
                            t.Result
                        })
                     .Select(group => new LCIAModel
                     {
                         LCIAResult = group.Sum(a => a.Result)
                     });
                        */
                        //ScoreCache scoreCache = new ScoreCache();
                        //scoreCache.ScenarioID = fragmentNode.ScenarioID;
                        //scoreCache.FragmentFlowID = fragmentFlowId;
                        //scoreCache.LCIAMethodID = lciaMethodItem.LCIAMethodID;
                        //scoreCache.ImpactScore = Convert.ToDouble(lcias.Sum(a => a.Result));
                        //scoreCache.ObjectState = ObjectState.Added;
                        //_scoreCacheService.InsertOrUpdateGraph(scoreCache);

                        fragmentScoreCaches.Add(new ScoreCache()
                        {
                            ScenarioID = fragmentNode.ScenarioID,
                            FragmentFlowID = fragmentFlowId,
                            LCIAMethodID = lciaMethodItem.LCIAMethodID,
                            ImpactScore = Convert.ToDouble(lcias.Sum(a => a.Result))
                        });
                    }

                    foreach (var fragmentScoreCache in fragmentScoreCaches)
                    {
                        fragmentScoreCache.ObjectState = ObjectState.Added;
                    }
                    _scoreCacheService.InsertGraphRange(fragmentScoreCaches);

                    break;
            }  /* end of switch NodeType */

            _unitOfWork.SaveChanges();
            //enable this after results have been cached -  it was turned off to increase performance
            _unitOfWork.SetAutoDetectChanges(true);
        } /* end of SetScoreCache */
    }
}
