using Entities.Models;
using LcaDataModel;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FragmentLCIAComputation
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

        public FragmentLCIAComputation(IService<FragmentFlow> fragmentFlowService,
            IService<ScoreCache> scoreCacheService,
            IService<NodeCache> nodeCacheService,
            IService<FragmentNodeProcess> fragmentNodeProcessService,
            IService<ProcessSubstitution> processSubstitutionService,
            IService<FragmentNodeFragment> fragmentNodeFragmentService,
            IService<FragmentSubstitution> fragmentSubstitutionService,
            IService<LCIAMethod> lciaMethodService,
            IService<ScenarioBackground> scenarioBackgroundService,
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
            IService<Param> paramService)
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
        }

        public IEnumerable<FragmentLCIAModel> GetLCIAMethodsForComputeFragmentLCIA()
        {
            var lciaMethods = _lciaMethodService.Query().Get();
            var c = FragmentFlowLCIA(11, 1, lciaMethods);
            return c.ToList();

        }

        public IEnumerable<FragmentLCIAModel> ComputeFragmentLCIA(int fragmentId, int scenarioId, int lciaMethodId)
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

        public IEnumerable<FragmentLCIAModel> FragmentFlowLCIA(int fragmentId, int scenarioId, IEnumerable<LCIAMethod> lciaMethods)
        {
            // set score cache for fragment / scenario / method: iterate through
            // fragmentflows 

            var fragmentFlows = _fragmentFlowService.Query().Get()
                .Where(x => x.FragmentID == fragmentId);

            foreach (var item in fragmentFlows)
            {
                int nodeCacheId = Convert.ToInt32(_nodeCacheService.Query().Get()
                    .Where(x => x.FragmentFlowID == Convert.ToInt32(item.FragmentFlowID))
                    .Where(x => x.ScenarioID == scenarioId));

                int nodeTypeId = Convert.ToInt32(item.NodeTypeID);
                int targetId;
                switch (nodeTypeId)
                {
                    case 1:
                        var target1 = _fragmentNodeProcessService.Query().Get()
                            .Where(x => x.FragmentFlowID == item.FragmentFlowID)
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
      });
                        foreach (var target1Item in target1)
                        {
                            if (target1Item.psProcessID == 0)
                            {
                                target1Item.psProcessID = target1Item.fnpProcessID;
                            }
                        }

                        targetId = Convert.ToInt32(target1.Select(x => x.psProcessID).FirstOrDefault());

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
 
                        var scores = lciaComputation.ProcessLCIA(targetId, lciaMethods, scenarioId);
                        foreach (var lciaMethodItem in lciaMethods)
                        {
                            SetScoreCache(nodeCacheId, lciaMethodItem.LCIAMethodID, scores.Select(x => x.Result).FirstOrDefault());
                        }

    //                    score[...] = ProcessLCIA( target_id, scenario_id, lciamethod_ids);
    //for j=1:length(lciamethod_ids)
    //{
    //  SetScoreCache( i.NodeCacheID, lciamehod_ids[j], score[j]);
    //}

                        break;
                    case 2:

                        var target2 = _fragmentNodeFragmentService.Query().Get()
                            .Where(x => x.FragmentFlowID == item.FragmentFlowID)
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
                        targetId = ResolveBackground(item.FlowID, item.DirectionID, scenarioId, nodeTypeId);
                        break;
                }

            }


        }

//        target_id = ResolveBackground ( flow_id, direction_id, scenario_id, &node_type_id )
//{
//  // resolve a given flow to a particular background system
//  // first: check to see if there is a scenario-specific value
//  B = SELECT NodeTypeID, TargetID FROM ScenarioBackground
//      WHERE ScenarioID = scenario_id
//        AND FlowID = flow_id
//    AND DirectionID = direction_id;

//  if isempty(B)
//  {
//    B = SELECT NodeTypeID, TargetID FROM Background
//        WHERE FlowID = flow_id
//      AND DirectionID = direction_id;

//    if isempty(B)
//      catch exception!
//  }

//  // this is supposed to update the value of the reference
//  node_type_id = B.NodeTypeID;

//  if node_type_id == 5
//    target_id = 0;
//  else
//    target_id = B.TargetID;
//}

        public int? ResolveBackground(int flowId, int directionId, int scenarioId, int nodeTypeId)
        {
            IEnumerable<ResolveBackgroundModel> background = null;

            background = _scenarioBackgroundService.Query().Get()
                .Where(x => x.ScenarioID == scenarioId)
                .Where(x => x.FlowID == flowId)
                //this doesn't exist in the table.  Wait for it to be added
                //.Where(x => x.DirectionID == directionId)
               .Select(bg => new ResolveBackgroundModel
               {
                   NodeTypeID = bg.NodeTypeID,
                   TargetID = bg.TargetID
               })
              



        }

    }
}
