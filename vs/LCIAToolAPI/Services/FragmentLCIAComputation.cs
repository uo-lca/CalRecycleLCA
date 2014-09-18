using Entities.Models;
using LcaDataModel;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FragmentLCIAComputation : IFragmentLCIAComputation
    {
        [Inject]
        private readonly IService<FragmentFlow> _fragmentFlowService;
        [Inject]
        private readonly IService<NodeCache> _nodeCacheService;
        [Inject]
        private readonly IService<ScoreCache> _scoreCacheService;
        [Inject]
        private readonly IService<FragmentNodeProcess> _fragmentNodeProcessService;
        [Inject]
        private readonly IService<ProcessSubstitution> _processSubstitutionService;
        [Inject]
        private readonly IService<FragmentNodeFragment> _fragmentNodeFragmentService;
        [Inject]
        private readonly IService<FragmentSubstitution> _fragmentSubstitutionService;
        [Inject]
        private readonly IService<LCIAMethod> _lciaMethodService;
        [Inject]
        private readonly IService<ScenarioBackground> _scenarioBackgroundService;
        [Inject]
        private readonly IService<Background> _backgroundService;
        [Inject]
        private readonly IService<ProcessFlow> _processFlowService;
        [Inject]
        private readonly IService<ProcessEmissionParam> _processEmissionParamService;
        [Inject]
        private readonly IService<Flow> _flowService;
        [Inject]
        private readonly IService<FlowFlowProperty> _flowFlowPropertyService;
        [Inject]
        private readonly IService<FlowPropertyParam> _flowPropertyParamService;
        [Inject]
        private readonly IService<FlowPropertyEmission> _flowPropertyEmissionService;
        [Inject]
        private readonly IService<ProcessDissipation> _processDissipationService;
        [Inject]
        private readonly IService<ProcessDissipationParam> _processDissipationParamService;
        [Inject]
        private readonly IService<LCIA> _lciaService;
        [Inject]
        private readonly IService<CharacterizationParam> _characterizationParamService;
        [Inject]
        private readonly IService<Param> _paramService;
        [Inject]
        private readonly IService<DependencyParam> _dependencyParamService;
        [Inject]
        private readonly IService<Fragment> _fragmentService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;
        [Inject]
        private readonly IUnitOfWork _unitOfWork1;

        public FragmentLCIAComputation(IService<FragmentFlow> fragmentFlowService,
            IService<ScoreCache> scoreCacheService,
            IService<NodeCache> nodeCacheService,
            IService<FragmentNodeProcess> fragmentNodeProcessService,
            IService<ProcessSubstitution> processSubstitutionService,
            IService<FragmentNodeFragment> fragmentNodeFragmentService,
            IService<FragmentSubstitution> fragmentSubstitutionService,
            IService<LCIAMethod> lciaMethodService,
            IService<ScenarioBackground> scenarioBackgroundService,
            IService<Background> backgroundService,
            IService<ProcessFlow> processFlowService,
            IService<ProcessEmissionParam> processEmissionParamService,
            IService<Flow> flowService,
            IService<FlowFlowProperty> flowFlowPropertyService,
            IService<FlowPropertyParam> flowPropertyParamService,
            IService<FlowPropertyEmission> flowPropertyEmissionService,
            IService<ProcessDissipation> processDissipationService,
            IService<ProcessDissipationParam> processDissipationParamService,
            IService<LCIA> lciaService,
            IService<CharacterizationParam> characterizationParamService,
            IService<Param> paramService,
            IService<DependencyParam> dependencyParamService,
            IService<Fragment> fragmentService,
            IUnitOfWork unitOfWork,
            IUnitOfWork unitOfWork1)
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

            if (unitOfWork1 == null)
            {
                throw new ArgumentNullException("unitOfWork1 is null");
            }
            _unitOfWork1 = unitOfWork1;
        }

        public void FragmentLCIACompute(int fragmentId, int scenarioId)
        {
            var lciaMethods = _lciaMethodService.Query().Get().ToList();
            var c = FragmentFlowLCIA(fragmentId, scenarioId, lciaMethods).ToList();
            //return c.ToList();

        }

        public IEnumerable<FragmentLCIAModel> ComputeFragmentLCIA(int? fragmentId, int? scenarioId, int? lciaMethodId)
        {
            // for now: return one record per FragmentFlow
            var lcia = _fragmentFlowService.Query().Get()
                .Where(x => x.FragmentID == fragmentId)
                .Join(_nodeCacheService.Query().Get(), ff => ff.FragmentFlowID, nc => nc.FragmentFlowID, (ff, nc) => new { ff, nc })
                .Where(x => x.nc.ScenarioID == scenarioId)
                .GroupJoin(_scoreCacheService.Query().Get()
                // leave this out for now, as you obviously can't add a
                // where clause for a table that has no data.
                //.Where(x => x.LCIAMethodID == lciaMethodId)
      , l => l.nc.NodeCacheID
      , sc => sc.NodeCacheID
      , (nc, sc) => new { nodeCaches = nc, scoreCaches = sc })
      .SelectMany(s => s.scoreCaches.DefaultIfEmpty()
      , (s, scoreCaches) => new FragmentLCIAModel
      {
          FragmentFlowID = s.nodeCaches.ff.FragmentFlowID,
          NodeWeight = s.nodeCaches.nc.NodeWeight,
          ImpactScore = scoreCaches.ImpactScore
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

            var fragmentFlows = _nodeCacheService.Query().Get()
                .Where(x => x.ScenarioID == scenarioId)
                .Join(_fragmentFlowService.Query().Get(), nc => nc.FragmentFlowID, ff => ff.FragmentFlowID, (nc, ff) => new { nc, ff })
                .Where(x => x.ff.FragmentID == fragmentId).ToList();

            foreach (var item in fragmentFlows)
            {
                var nodeCache = _nodeCacheService.Query().Get()
                    .Where(x => x.FragmentFlowID == Convert.ToInt32(item.ff.FragmentFlowID))
                    .Where(x => x.ScenarioID == scenarioId);

                int nodeCacheId = Convert.ToInt32(nodeCache.Select(x => x.NodeCacheID).FirstOrDefault());

                int nodeTypeId = Convert.ToInt32(item.ff.NodeTypeID);
                int? targetId = 0;
                switch (nodeTypeId)
                {
                    case 1:
                        var target1 = _fragmentNodeProcessService.Query().Get()
                            .Where(x => x.FragmentFlowID == item.ff.FragmentFlowID)
                                  .GroupJoin(_processSubstitutionService.Query().Get()
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

                        var score = lciaComputation.ProcessLCIA(targetId, lciaMethods, scenarioId);
                        
                        IList<ScoreCache> scoreCaches = new List<ScoreCache>();
                        foreach (var lciaMethodItem in lciaMethods.AsQueryable())
                       {
                           SetScoreCache(nodeCacheId, lciaMethodItem.LCIAMethodID, score);
                         
                        }
                        _unitOfWork.Save();

                        break;
                    case 2:

                        var target2 = _fragmentNodeFragmentService.Query().Get()
                            .Where(x => x.FragmentFlowID == item.ff.FragmentFlowID)
                                  .GroupJoin(_fragmentSubstitutionService.Query().Get()
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
      });

                        foreach (var target2Item in target2)
                        {
                            if (target2Item.psSubFragmentID == 0)
                            {
                                target2Item.psSubFragmentID = target2Item.fnpSubFragmentID;
                            }
                        }

                        targetId = Convert.ToInt32(target2.Select(x => x.psSubFragmentID).FirstOrDefault());

                        //recursive LCIA computation, results to cache
                        FragmentFlowLCIA(targetId, scenarioId, lciaMethods);

                        foreach (var lciaMethodItem in lciaMethods)
                        {
                            var lcias = ComputeFragmentLCIA(targetId, scenarioId, lciaMethodItem.LCIAMethodID);
                            var lciaScore = lcias.ToList()
                        .GroupBy(t => new
                        {
                            t.ImpactScore
                        })
                     .Select(group => new LCIAModel
                     {
                         Result = group.Sum(a => a.ImpactScore)
                     });
                            SetScoreCache(nodeCacheId, lciaMethodItem.LCIAMethodID, lciaScore.Select(x => x.Result).FirstOrDefault());
                        }


                        break;
                    case 3:
                        //do nothing for Input/Output
                        break;

                    case 4:
                        targetId = ResolveBackground(item.ff.FlowID, item.ff.DirectionID, scenarioId, nodeTypeId);
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


        public int ResolveBackground(int? flowId, int? directionId, int? scenarioId, int? nodeTypeId)
        {
            IEnumerable<ResolveBackgroundModel> background = null;
            int targetId;
            background = _scenarioBackgroundService.Query().Get()
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
                background = _backgroundService.Query().Get()
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

            return targetId;

        }

        public void SetScoreCache(int? nodeCacheId, int? lciaMethodId, double? result)
        {
            ScoreCache scoreCache = new ScoreCache();
            scoreCache.NodeCacheID = nodeCacheId;
            scoreCache.LCIAMethodID = lciaMethodId;
            scoreCache.ImpactScore = result;
            _scoreCacheService.InsertGraph(scoreCache);
        }

    }
}
