using Data;
using Entities.Models;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FragmentTraversal : IFragmentTraversal
    {
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly IDependencyParamService _dependencyParamService;
        [Inject]
        private readonly IScenarioParamService _scenarioParamService;
        [Inject]
        private readonly IParamService _paramService;
        int fragmentId = 1;

        public FragmentTraversal(IFragmentFlowService fragmentFlowService,
            IDependencyParamService dependencyParamService,
            IScenarioParamService scenarioParamService,
            IParamService paramService)
        {

            if (fragmentFlowService == null)
            {
                throw new ArgumentNullException("fragmentFlowService is null");
            }

            _fragmentFlowService = fragmentFlowService;

            if (dependencyParamService == null)
            {
                throw new ArgumentNullException("dependencyParamService is null");
            }

            _dependencyParamService = dependencyParamService;

            if (scenarioParamService == null)
            {
                throw new ArgumentNullException("ScenarioParamService is null");
            }

            _scenarioParamService = scenarioParamService;

            if (paramService == null)
            {
                throw new ArgumentNullException("paramService is null");
            }

            _paramService = paramService;


        }

        public IEnumerable<DependencyParamModel> ApplyDependencyParam(int scenarioId = 0)
        {
            //var unitOfWork = new UnitOfWork();

            //get fragment flows by fragmentId
            var fragmentFlows = _fragmentFlowService.Query().Filter(q => q.FragmentID == fragmentId).Get().ToList();
            //return fragmentFlows.Get().ToList();

            //get scenario params by id
            var scenarioParams = _scenarioParamService.Query().Filter(q => q.ScenarioID == scenarioId).Get().ToList();

            //get params - can't name it params as it's a reserved word
            var parameters = _paramService.Query().Get().ToList();
            //return parameters.Get().ToList();

            //get dependencyParams
            var dependencyParams = _dependencyParamService.Query().Get().ToList();
            //return dependencyParams.Get().ToList();

            //get scenario specific params
            var query = scenarioParams
       .Join(parameters, p => p.ParamID, pc => pc.ParamID, (p, pc) => new { p, pc })
       .Join(dependencyParams, ppc => ppc.pc.ParamID, c => c.ParamID, (ppc, c) => new { ppc, c })
       .Select(m => new DependencyParam
       {
           ParamID = m.ppc.p.ParamID, // or m.ppc.pc.ProdId
           FragmentFlowID = m.c.FragmentFlowID,
           Value = m.c.Value
       }).ToList();



            //return query.AsEnumerable();

                    // generate scenario-specific edge table.  The idea is that we use the
                    // edge weights specified in the default FragmentEdge table, unless they
                    // have been overridden by scenario params
                    var result = fragmentFlows.GroupJoin(query,
                ff => ff.FragmentFlowID,
                dp => dp.FragmentFlowID,
                (ff, dp) =>
                    new { FragmentFlow = ff, DependencyParam = dp.DefaultIfEmpty() })
            .SelectMany(a => a.DependencyParam.
                Select(b => new { FragmentFlowID = a.FragmentFlow.FragmentFlowID, 
                                  FragmentID=a.FragmentFlow.FragmentID, 
                                  Name=a.FragmentFlow.Name, 
                                  FragmentStageID=a.FragmentFlow.FragmentStageID ?? 0,
                                  ReferenceFlowPropertyID = a.FragmentFlow.ReferenceFlowPropertyID,
                                  NodeTypeID = a.FragmentFlow.NodeTypeID,
                                  FlowID = a.FragmentFlow.FlowID,
                                  DirectionID = a.FragmentFlow.DirectionID,
                                  Quantity = a.FragmentFlow.Quantity,
                                  ParentFragmentFlowID = a.FragmentFlow.ParentFragmentFlowID,
                                  Value = (b == null ? Double.NaN : b.Value) })).ToList();

                    return result
                .Select(ic => new DependencyParamModel
                {
                    FragmentFlowID = ic.FragmentFlowID,
                    FragmentID = ic.FragmentID,
                    Name = ic.Name,
                    FragmentStageID = ic.FragmentStageID,
                    ReferenceFlowPropertyID = ic.ReferenceFlowPropertyID,
                    NodeTypeID = ic.NodeTypeID,
                    FlowID = ic.FlowID,
                    DirectionID = ic.DirectionID,
                    Quantity = ic.Quantity,
                    ParentFragmentFlowID = ic.ParentFragmentFlowID,
                    Value = ic.Value
                })
            .AsQueryable<DependencyParamModel>();
                   


        }


    }
}
