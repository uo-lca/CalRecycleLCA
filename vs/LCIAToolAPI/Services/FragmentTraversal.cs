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
        [Inject]
        private readonly IFlowFlowPropertyService _flowFlowPropertyService;

        int fragmentId = 1;

        public FragmentTraversal(IFragmentFlowService fragmentFlowService,
            IDependencyParamService dependencyParamService,
            IScenarioParamService scenarioParamService,
            IParamService paramService, IFlowFlowPropertyService flowFlowPropertyService)
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

            if (flowFlowPropertyService == null)
            {
                throw new ArgumentNullException("flowFlowPropertyService is null");
            }

            _flowFlowPropertyService = flowFlowPropertyService;


        }

        public IEnumerable<DependencyParamModel> ApplyDependencyParam(int scenarioId = 0)
        {

            //get fragment flows by fragmentId
            var fragmentFlows = _fragmentFlowService.Query().Filter(q => q.FragmentID == fragmentId).Get().ToList();

            //get scenario params by id
            var scenarioParams = _scenarioParamService.Query().Filter(q => q.ScenarioID == scenarioId).Get().ToList();

            //get params - can't name it params as it's a reserved word
            var parameters = _paramService.Query().Get().ToList();

            //get dependencyParams
            var dependencyParams = _dependencyParamService.Query().Get().ToList();

            //get scenario specific params
            var query = scenarioParams
           .Join(parameters, p => p.ParamID, pc => pc.ParamID, (p, pc) => new { p, pc })
           .Join(dependencyParams, ppc => ppc.pc.ParamID, c => c.ParamID, (ppc, c) => new { ppc, c })
           .Select(m => new DependencyParam
           {
               ParamID = m.ppc.p.ParamID,
               FragmentFlowID = m.c.FragmentFlowID,
               Value = m.c.Value
           }).ToList();

            // generate scenario-specific edge table.  The idea is that we use the
            // edge weights specified in the default FragmentEdge table, unless they
            // have been overridden by scenario params
            var result = fragmentFlows.GroupJoin(query,
            ff => ff.FragmentFlowID,
            dp => dp.FragmentFlowID,
            (ff, dp) =>
                new { FragmentFlow = ff, DependencyParam = dp.DefaultIfEmpty() })
                    .SelectMany(a => a.DependencyParam.
                    Select(b => new
                        {
                            FragmentFlowID = a.FragmentFlow.FragmentFlowID,
                            FragmentID = a.FragmentFlow.FragmentID,
                            Name = a.FragmentFlow.Name,
                            FragmentStageID = a.FragmentFlow.FragmentStageID ?? 0,
                            ReferenceFlowPropertyID = a.FragmentFlow.ReferenceFlowPropertyID,
                            NodeTypeID = a.FragmentFlow.NodeTypeID,
                            FlowID = a.FragmentFlow.FlowID,
                            DirectionID = a.FragmentFlow.DirectionID,
                            Quantity = a.FragmentFlow.Quantity,
                            ParentFragmentFlowID = a.FragmentFlow.ParentFragmentFlowID,
                            Value = (b == null ? 0 : b.Value)
                        })).ToList();

            //select them into a model so that the list can be updated.  
            //Anonymous types are read only
            var edges = result
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
            .ToList<DependencyParamModel>();

            //take the param values as defaults:
            var defaults = edges
           .Where(p => p.Value != 0);

            //loop through list of edges and update quantity to value where a value is present for value 
            foreach (var item in defaults)
            {
                item.Quantity = item.Value;
            }

            edges.RemoveAll(p => p.Value != 0);

            //return updated list of edges
            return edges
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

        public IEnumerable<FlowPropertyParamModel> ApplyFlowPropertyParam(int scenarioId = 0)
        {

            //get fragment flows by fragmentId
            var fragmentFlows = _fragmentFlowService.Query().Filter(q => q.FragmentID == fragmentId).Get().ToList();

            //get flow flow Properties
            var flowFlowProperties = _flowFlowPropertyService.Query().Get().ToList();

             //get params - can't name it params as it's a reserved word
            var parameters = _paramService.Query().Get().ToList();

            //get scenario params by id
            var scenarioParams = _scenarioParamService.Query().Filter(q => q.ScenarioID == scenarioId).Get().ToList();

            // first, determine the correct FlowFlowPropertyID
            var query = fragmentFlows
           .Join(flowFlowProperties, p => p.FlowID, pc => pc.FlowID, (p, pc) => new { p, pc })
           .Where(p => p.p.ReferenceFlowPropertyID == p.pc.FlowPropertyID)
           .Select(m => new FlowPropertyParamModel
           {
               FragmentFlowID = m.p.FragmentFlowID,
               FragmentID = m.p.FragmentID,
               Name = m.p.Name,
               FragmentStageID = m.p.FragmentStageID ?? 0,
               ReferenceFlowPropertyID = m.p.ReferenceFlowPropertyID,
               NodeTypeID = m.p.NodeTypeID,
               FlowID = m.p.FlowID,
               DirectionID = m.p.DirectionID,
               Quantity = m.p.Quantity,
               ParentFragmentFlowID = m.p.ParentFragmentFlowID,
               MeanValue = (m.pc.MeanValue == null ? 0 : m.pc.MeanValue)
           }).AsEnumerable();

            return query;



        }
    }
}
