using Entities.Models;
using LcaDataModel;
using Ninject;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System;
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
        [Inject]
        private readonly IFragmentNodeProcessService _fragmentNodeProcessService;
        [Inject]
        private readonly IProcessSubstitutionService _processSubstitutionService;
        [Inject]
        private readonly IFragmentNodeFragmentService _fragmentNodeFragmentService;
        [Inject]
        private readonly IFragmentSubstitutionService _fragmentSubstitutionService;
        [Inject]
        private readonly ILCIAMethodService _lciaMethodService;
        [Inject]
        private readonly IScenarioBackgroundService _scenarioBackgroundService;
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
        private readonly IUnitOfWork _unitOfWork;
     
        public FragmentLCIAComputation(IFragmentFlowService fragmentFlowService,
            IScoreCacheService scoreCacheService,
            INodeCacheService nodeCacheService,
            IFragmentNodeProcessService fragmentNodeProcessService,
            IProcessSubstitutionService processSubstitutionService,
            IFragmentNodeFragmentService fragmentNodeFragmentService,
            IFragmentSubstitutionService fragmentSubstitutionService,
            ILCIAMethodService lciaMethodService,
            IScenarioBackgroundService scenarioBackgroundService,
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

            if (scenarioBackgroundService == null)
            {
                throw new ArgumentNullException("scenarioBackgroundService is null");
            }
            _scenarioBackgroundService = scenarioBackgroundService;


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

            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork is null");
            }
            _unitOfWork = unitOfWork;
        }

        public void FragmentLCIACompute(int fragmentId, int scenarioId)
        {

            var lciaMethods = _lciaMethodService.Queryable().ToList();

//select distinct f.FragmentID, n.ScenarioID from NodeCache n inner join ScoreCache s
//on n.NodeCacheID = s.NodeCacheID
//inner join FragmentFlow f on f.FragmentFlowID =n.FragmentFlowID
//where FragmentID=3 and ScenarioID=0

            // //// [BK] Moving cache checking into SetScoreCache
            //var scoreCache = _scoreCacheService.Query().Get()
            //    .Join(_nodeCacheService.Query().Filter(x => x.ScenarioID == scenarioId).Get(), sc => sc.NodeCacheID, nc => nc.NodeCacheID, (sc, nc) => new { sc, nc })
            //    .Join(_fragmentFlowService.Query().Filter(x => x.FragmentID == fragmentId).Get(), nc => nc.nc.FragmentFlowID, ff => ff.FragmentFlowID, (nc, ff) => new { nc, ff });

            
            //if (scoreCache.Count() == 0)
            //{
                FragmentFlowLCIA(fragmentId, scenarioId, lciaMethods).ToList();
            //}
          

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
      , (nc, sc) => new { nodeCaches = nc, scoreCaches = sc }).Select( s => new FragmentLCIAModel
            {
                FragmentFlowID = s.nodeCaches.ff.FragmentFlowID,
                NodeWeight = s.nodeCaches.nc.NodeWeight,
                ImpactScore = s.scoreCaches == null ? 0 : s.scoreCaches.ImpactScore,
	            Result = s.nodeCaches.nc.NodeWeight * (s.scoreCaches == null ? 0 : s.scoreCaches.ImpactScore)
            });

            return lcia;
        }

        public IEnumerable<FragmentLCIAModel> FragmentFlowLCIA(int? fragmentId, int scenarioId, IEnumerable<LCIAMethod> lciaMethods)
        {
            // set score cache for fragment / scenario / method: iterate through
            // fragmentflows 


            // this does nothing if traversal has already been completed - FIGURE OUT THIS PART ON THURSDAY
            FragmentTraversalV2 fragmentTraversalV2 = new FragmentTraversalV2(_flowService,
                            _fragmentFlowService,
                            _nodeCacheService,
                            _fragmentNodeProcessService,
                            _processFlowService,
                            _fragmentNodeFragmentService,
                            _flowFlowPropertyService,
                            _dependencyParamService,
                            _flowPropertyParamService,
                            _fragmentService,
                            _paramService,
                            _unitOfWork);

            fragmentTraversalV2.Traverse(fragmentId, scenarioId);

            var fragmentFlows = _nodeCacheService.Queryable()
                .Where(x => x.ScenarioID == scenarioId)
                .Join(_fragmentFlowService.Queryable().Where(x => x.FragmentID == fragmentId), nc => nc.FragmentFlowID, ff => ff.FragmentFlowID, (nc, ff) => new { nc, ff }).ToList();
            
            foreach (var item in fragmentFlows)
            {
                // var nodeCache = _nodeCacheService.Query().Filter(x => x.FragmentFlowID == item.ff.FragmentFlowID && x.ScenarioID == scenarioId).Get();

                // int nodeCacheId = Convert.ToInt32(nodeCache.Select(x => x.NodeCacheID).FirstOrDefault());

                int nodeTypeId = Convert.ToInt32(item.ff.NodeTypeID);
                int? targetId = 0;
                switch (nodeTypeId)
                {
                    case 1:
                        var target1 = _fragmentNodeProcessService.Queryable()
                            .Where(x => x.FragmentFlowID == item.ff.FragmentFlowID)
                                  .GroupJoin(_processSubstitutionService.Queryable()
                            //leave this out for now, as you obviously can't add a
                            //where clause for a table that has no data.
                            //.Where(x => x.ScenarioID == scenarioId)
      , fnp => fnp.ProcessID
      , ps => ps.ProcessID
      , (fnp, ps) => new { fragmentNodeProcesses = fnp, processSubstitutions = ps })
      .SelectMany(x => x.processSubstitutions.DefaultIfEmpty()
      , (x, processSubstitutions) => new FragmentLCIAModel
      {
          fnpProcessID = x.fragmentNodeProcesses.ProcessID,
          psProcessID = processSubstitutions == null ? 0 : processSubstitutions.ProcessID
      }).ToList();
                        foreach (var target1Item in target1)
                        {
                            if (target1Item.psProcessID == 0)
                            {
                                targetId = target1Item.fnpProcessID;
                            }
                            else
                            {
                                targetId = target1Item.psProcessID;
                            }
                        }

                        SetScoreCache(targetId, nodeTypeId, lciaMethods, scenarioId, item.ff.FragmentFlowID);

                        break;
                    case 2:

                        var target2 = _fragmentNodeFragmentService.Queryable()
                            .Where(x => x.FragmentFlowID == item.ff.FragmentFlowID)
                                  .GroupJoin(_fragmentSubstitutionService.Queryable()
                            //leave this out for now, as you obviously can't add a
                            //where clause for a table that has no data.
                            //.Where(x => x.ScenarioID == scenarioId)
      , fnf => fnf.SubFragmentID
      , fs => fs.SubFragmentID
      , (fnf, fs) => new { fragmentNodeFragments = fnf, fragmentSubstitutions = fs })
      .SelectMany(x => x.fragmentSubstitutions.DefaultIfEmpty()
      , (x, fragmentSubstitutions) => new FragmentLCIAModel
      {
          fnpSubFragmentID = x.fragmentNodeFragments.SubFragmentID,
          psSubFragmentID = fragmentSubstitutions == null ? 0 : fragmentSubstitutions.SubFragmentID
      }).ToList();

                        foreach (var target2Item in target2)
                        {

                            if (target2Item.psSubFragmentID == 0)
                            {
                                targetId = target2Item.fnpSubFragmentID;
                            }
                            else
                            {
                                targetId = target2Item.psSubFragmentID;
                            }
                        }



                        //recursive LCIA computation, results to cache
                        FragmentFlowLCIA(targetId, scenarioId, lciaMethods);
                        SetScoreCache(targetId, nodeTypeId, lciaMethods, scenarioId, item.ff.FragmentFlowID);
                       
                        break;
                    case 3:
                        //do nothing for Input/Output
                        break;

                    case 4:
                        //needed to get an object containing NodetypeId and targetId - not just the integer targetid
                        var resolveBackground = ResolveBackground(item.ff.FlowID, item.ff.DirectionID, scenarioId, nodeTypeId);
                        int? resolveBackgroundTargetId = resolveBackground.TargetID;
                        int? resolveBackgroundNodeTypeId = resolveBackground.NodeTypeID;
                        SetScoreCache(resolveBackgroundTargetId, resolveBackgroundNodeTypeId, lciaMethods, scenarioId, item.ff.FragmentFlowID);
                        break;
                }

            }

            return fragmentFlows
                  .Select(x => new FragmentLCIAModel
                  {
                      FragmentFlowID = x.ff.FragmentFlowID,
                      NodeTypeID = x.ff.NodeTypeID,
                      FlowID = x.ff.FlowID,
                      DirectionID = x.ff.DirectionID
                  });
        }


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
                   TargetID = bg.ILCDEntityID
               });

            if (background.Count() == 0)
            {
                background = _backgroundService.Queryable()
                .Where(x => x.FlowID == flowId)
                .Where(x => x.DirectionID == directionId)
               .Select(bg => new ResolveBackgroundModel
               {
                   NodeTypeID = bg.NodeTypeID,
                   TargetID = bg.ILCDEntityID
               });

                if (background.Count() == 0)
                {
                    throw new ArgumentNullException("background is null");
                }
            }

            nodeTypeId = Convert.ToInt32(background.Select(x => x.NodeTypeID).FirstOrDefault());

            if (nodeTypeId == 5)
            {
                targetId = 0;
            }
            else
            {
                targetId = Convert.ToInt32(background.Select(x => x.TargetID).FirstOrDefault());
            }

            //return targetId;

            ResolveBackgroundModel resolveBackground = new ResolveBackgroundModel();
            resolveBackground.NodeTypeID = nodeTypeId;
            resolveBackground.TargetID = targetId;
            return resolveBackground;

        }


        public void SetScoreCache(int? targetId, int? nodeTypeId, IEnumerable<LCIAMethod> lciaMethods, int scenarioId, int fragmentFlowId)
        {
            IEnumerable<LCIAMethod> haveLciaMethods = _scoreCacheService.Queryable().Where(x => x.ScenarioID == scenarioId
                                                                                        && x.FragmentFlowID == fragmentFlowId).AsEnumerable()
                                                                                             .Select(y => new LCIAMethodResource
                                                                                             { 
                                                                                             LCIAMethodID = y.LCIAMethodID })
                                                                                              .Select(y => new LCIAMethod
                                                                                              {
                                                                                                  LCIAMethodID = y.LCIAMethodID
                                                                                              });
            
            IEnumerable<LCIAMethod> needLciaMethods = lciaMethods.Except(haveLciaMethods);

            if (needLciaMethods == null)
                return;

            switch (nodeTypeId)
            {
                case 1:
                    
                        LCIAComputationV2 lciaComputation = new LCIAComputationV2(_processFlowService,
                            _processEmissionParamService,
                            _lciaMethodService,
                            _flowService,
                            _flowFlowPropertyService,
                            _flowPropertyParamService,
                            _flowPropertyEmissionService,
                            _processDissipationService,
                            _processDissipationParamService,
                            _lciaService,
                            _characterizationParamService,
                            _paramService);

                        var scores = lciaComputation.ProcessLCIA(targetId, needLciaMethods, scenarioId);

                        IList<ScoreCache> scoreCaches = new List<ScoreCache>();

                        foreach (var lciaMethodItem in needLciaMethods.AsQueryable())
                        {
                            double impactScore = 0;
                            if (scores.Any(s => s.LCIAMethodID == lciaMethodItem.LCIAMethodID))
                            {
                                var score = scores
                                    .Where(x => x.LCIAMethodID == lciaMethodItem.LCIAMethodID)
                                    .Select(x => new ScoreCache
                                        {
                                            ImpactScore = Convert.ToDouble(x.Score)
                                        }).FirstOrDefault();

                                impactScore = Convert.ToDouble(score.ImpactScore);

                                ScoreCache scoreCache = new ScoreCache();
                                scoreCache.ScenarioID = scenarioId;
                                scoreCache.FragmentFlowID = fragmentFlowId;
                                scoreCache.LCIAMethodID = lciaMethodItem.LCIAMethodID;
                                scoreCache.ImpactScore = impactScore;
                                scoreCache.ObjectState = ObjectState.Added;
                                _scoreCacheService.InsertOrUpdateGraph(scoreCache);
                                
                            }

                        }
                        _unitOfWork.SaveChanges();
                    break;
                case 2:
                     foreach (var lciaMethodItem in lciaMethods)
                        {
                            var lcias = ComputeFragmentLCIA(targetId, scenarioId, lciaMethodItem.LCIAMethodID);

                            if (lcias != null)
                            {
                                var lciaScore = lcias.ToList()
                            .GroupBy(t => new
                            {
                                t.Result
                            })
                         .Select(group => new LCIAModel
                         {
                             Result = group.Sum(a => a.Result)
                         });

                                ScoreCache scoreCache = new ScoreCache();
                                scoreCache.ScenarioID = scenarioId;
                                scoreCache.FragmentFlowID = fragmentFlowId;
                                scoreCache.LCIAMethodID = lciaMethodItem.LCIAMethodID;
                                scoreCache.ImpactScore = Convert.ToDouble(lciaScore.Select(a => a.Result).FirstOrDefault());
                                scoreCache.ObjectState = ObjectState.Added;
                                _scoreCacheService.InsertOrUpdateGraph(scoreCache);
                            }
                        }

                        _unitOfWork.SaveChanges();

                    break;
            }
        }
    }
}
