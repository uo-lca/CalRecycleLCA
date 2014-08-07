using LcaDataModel;
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
        [Inject]
        private readonly IFlowPropertyParamService _flowPropertyParamService;
        [Inject]
        private readonly IFragmentService _fragmentService;
        [Inject]
        private readonly IFlowService _flowService;
        [Inject]
        private readonly INodeCacheService _nodeCacheService;
        [Inject]
        private readonly IFragmentNodeFragmentService _fragmentNodeFragmentService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;

        float? activity = 1;

        public FragmentTraversal(IFragmentFlowService fragmentFlowService,
            IDependencyParamService dependencyParamService,
            IScenarioParamService scenarioParamService,
            IParamService paramService,
            IFlowFlowPropertyService flowFlowPropertyService,
            IFlowPropertyParamService flowPropertyParamService,
            IFragmentService fragmentService,
            IFlowService flowService,
            INodeCacheService nodeCacheService,
            IFragmentNodeFragmentService fragmentNodeFragmentService,
            IUnitOfWork unitOfWork)
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

            if (flowPropertyParamService == null)
            {
                throw new ArgumentNullException("flowPropertyParamService is null");
            }

            _flowPropertyParamService = flowPropertyParamService;

            if (fragmentService == null)
            {
                throw new ArgumentNullException("fragmentService is null");
            }

            _fragmentService = fragmentService;

            if (flowService == null)
            {
                throw new ArgumentNullException("flowService is null");
            }

            _flowService = flowService;

            if (nodeCacheService == null)
            {
                throw new ArgumentNullException("nodeCacheService is null");
            }

            _nodeCacheService = nodeCacheService;

            if (fragmentNodeFragmentService == null)
            {
                throw new ArgumentNullException("fragmentNodeFragmentService is null");
            }

            _fragmentNodeFragmentService = fragmentNodeFragmentService;

            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork is null");
            }

            _unitOfWork = unitOfWork;
        }

        public void Traverse(int fragmentID, int scenarioId = 1)
        {

            ApplyDependencyParam(fragmentID, scenarioId);
            ApplyFlowPropertyParam(fragmentID, scenarioId);
            NodeRecurse(scenarioId, refFlow(fragmentID), activity, fragmentID);

        }

        public IEnumerable<DependencyParamModel> ApplyDependencyParam(int fragmentID, int scenarioId = 1)
        {

            //get fragment flows by fragmentId
            var fragmentFlows = _fragmentFlowService.Query().Filter(q => q.FragmentID == fragmentID).Get().ToList();

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
                            // ReferenceFlowPropertyID = a.FragmentFlow.ReferenceFlowPropertyID,
                            NodeTypeID = a.FragmentFlow.NodeTypeID,
                            FlowID = a.FragmentFlow.FlowID,
                            DirectionID = a.FragmentFlow.DirectionID,
                            // Quantity = a.FragmentFlow.Quantity,
                            ParentFragmentFlowID = a.FragmentFlow.ParentFragmentFlowID,
                            //Value = (b == null ? 0 : b.Value)
                            ParamValue = (b == null ? -1 : b.Value)
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
                    // ReferenceFlowPropertyID = ic.ReferenceFlowPropertyID,
                    ReferenceFlowPropertyID = null, // TODO : Remove from model or populate using new design
                    NodeTypeID = ic.NodeTypeID,
                    FlowID = ic.FlowID,
                    DirectionID = ic.DirectionID,
                    // Quantity = ic.Quantity,
                    ParentFragmentFlowID = ic.ParentFragmentFlowID,
                    ParamValue = ic.ParamValue
                })
            .ToList<DependencyParamModel>();

            //take the param values as defaults:
            var defaults = edges
           .Where(p => p.ParamValue != -1);

            //loop through list of edges and update quantity to value where a value is present for value 
            foreach (var item in defaults)
            {
                item.Quantity = item.ParamValue;
            }

            edges.RemoveAll(p => p.ParamValue != -1);

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

        public IEnumerable<FlowPropertyParamModel> ApplyFlowPropertyParam(int fragmentID, int scenarioId = 1)
        {

            //get fragment flows by fragmentId
            var fragmentFlows = _fragmentFlowService.Query().Filter(q => q.FragmentID == fragmentID).Get().ToList();

            //get flow flow Properties
            var flowFlowProperties = _flowFlowPropertyService.Query().Get().ToList();

            //get params - can't name it params as it's a reserved word
            var parameters = _paramService.Query().Get().ToList();

            //get scenario params by id
            var scenarioParams = _scenarioParamService.Query().Filter(q => q.ScenarioID == scenarioId).Get().ToList();

            // first, determine the correct FlowFlowPropertyID
            var flowPropertyFlows = ApplyDependencyParam(fragmentID, scenarioId)
           .Join(flowFlowProperties, p => p.FlowID, pc => pc.FlowID, (p, pc) => new { p, pc })
           .Where(p => p.p.ReferenceFlowPropertyID == p.pc.FlowPropertyID)
           .Select(m => new FlowPropertyParamModel
           {
               FragmentFlowID = m.p.FragmentFlowID,
               FragmentID = m.p.FragmentID,
               Name = m.p.Name,
               FragmentStageID = m.p.FragmentStageID ?? 0,
               ReferenceFlowPropertyID = m.p.ReferenceFlowPropertyID,
               FlowFlowPropertyID = m.pc.FlowFlowPropertyID,
               NodeTypeID = m.p.NodeTypeID,
               FlowID = m.p.FlowID,
               DirectionID = m.p.DirectionID,
               Quantity = m.p.Quantity,
               ParentFragmentFlowID = m.p.ParentFragmentFlowID,
               Value = m.p.Value,
               MeanValue = (m.pc.MeanValue == null ? m.pc.FlowID : m.pc.MeanValue)
               //MeanValue = (m.pc.MeanValue)
           }).AsEnumerable();

            //return query;

            //get flowPropertyParams
            var flowPropertyParams = _flowPropertyParamService.Query().Get().ToList();

            //get scenario specific params
            var scenarioSpecific = scenarioParams
           .Join(parameters, p => p.ParamID, pc => pc.ParamID, (p, pc) => new { p, pc })
           .Join(flowPropertyParams, ppc => ppc.pc.ParamID, c => c.ParamID, (ppc, c) => new { ppc, c })
           .Select(m => new FlowPropertyParam
           {
               ParamID = m.ppc.p.ParamID,
               FlowPropertyParamID = m.c.FlowPropertyParamID,
               FlowFlowPropertyID = m.c.FlowFlowPropertyID,
               Value = m.c.Value
           }).ToList();

            // generate scenario-specific edge table.  The idea is that we use the
            // edge weights specified in the default FragmentEdge table, unless they
            // have been overridden by scenario params
            var edges = flowPropertyFlows.GroupJoin(scenarioSpecific,
            ff => ff.FlowFlowPropertyID,
            dp => dp.FlowFlowPropertyID,
            (ff, dp) =>
                new { FlowPropertyParamModel = ff, FlowPropertyParam = dp.DefaultIfEmpty() })
                    .SelectMany(a => a.FlowPropertyParam.
                    Select(b => new FlowPropertyParamModel
                    {
                        FragmentFlowID =a.FlowPropertyParamModel.FragmentFlowID,
                        FragmentID = a.FlowPropertyParamModel.FragmentID,
                        Name = a.FlowPropertyParamModel.Name,
                        FragmentStageID = a.FlowPropertyParamModel.FragmentStageID ?? 0,
                        ReferenceFlowPropertyID = a.FlowPropertyParamModel.ReferenceFlowPropertyID,
                        NodeTypeID = a.FlowPropertyParamModel.NodeTypeID,
                        FlowID = a.FlowPropertyParamModel.FlowID,
                        DirectionID = a.FlowPropertyParamModel.DirectionID,
                        Quantity = a.FlowPropertyParamModel.Quantity,
                        ParentFragmentFlowID = a.FlowPropertyParamModel.ParentFragmentFlowID,
                        ParamValue = (b == null ? -1 : b.Value),
                        MeanValue = a.FlowPropertyParamModel.MeanValue,
                        FlowFlowPropertyID = a.FlowPropertyParamModel.FlowFlowPropertyID
                    })).ToList();

            //return result;

            //take the param values as defaults:
            var defaults = edges
           .Where(p => p.ParamValue != -1);

            //loop through list of edges and update quantity to value where a value is present for value 
            foreach (var item in defaults)
            {
                item.MeanValue = item.ParamValue;
            }

            edges.RemoveAll(p => p.ParamValue != -1);

            //return updated list of edges
            return edges
               .Select(ic => new FlowPropertyParamModel
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
                   MeanValue = ic.MeanValue,
                   FlowFlowPropertyID = ic.FlowFlowPropertyID
               })
           .AsQueryable<FlowPropertyParamModel>();

        }

        public int refFlow(int fragmentID)
        {
            return Convert.ToInt32(_fragmentService.Query().Filter(q => q.FragmentID == fragmentID).Get().FirstOrDefault().ReferenceFragmentFlowID);
        }

        public void NodeRecurse(int scenarioId, int refFlow, double? activity, int fragmentID)
        {
            var theflow = ApplyFlowPropertyParam(fragmentID, scenarioId).Where(q => q.FragmentFlowID == refFlow).AsEnumerable();

            var fragmentNodeFragment = _fragmentNodeFragmentService.Query().Get().AsQueryable();

            double? nodeWeight = theflow.Select(x => x.Quantity).FirstOrDefault();
            double? nodeConv = theflow.Select(x => x.MeanValue).FirstOrDefault();
            int? nodeTypeID = theflow.Select(x => x.NodeTypeID).FirstOrDefault();

            //changed activity to activityNodeWeight as code was resetting activity to 0
            activity = activity * nodeWeight * nodeConv;


            var nodeCache = new NodeCache
            {
                ScenarioID = scenarioId,
                FragmentFlowID = refFlow,
                NodeWeight = activity

            };
            _nodeCacheService.InsertGraph(nodeCache);
            _unitOfWork.Save();


            switch (nodeTypeID)
            {
                case 1: //process
                    var processDependencies = ApplyFlowPropertyParam(fragmentID, scenarioId).Where(q => q.ParentFragmentFlowID == refFlow);
                    foreach (var item in processDependencies)
                    {
                        NodeRecurse(1, item.FragmentFlowID, activity, fragmentID);
                    }
                    break;
                case 2: //fragment
                    var theNode = theflow.Join(fragmentNodeFragment, p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc }).AsQueryable();
                    // flows in current fragment that depend on this node

                    //get the fragmentFlowId from the node
                    int? currentFlow = theNode.Select(x => x.p.FragmentFlowID).FirstOrDefault();
                    //get the subFragmentId from the node
                    int? subFragmentId = theNode.Select(x => x.pc.SubFragmentID).FirstOrDefault();

                    //get the dependencies associated with the node from the flows (these come from the flows after "ApplyDependencyParam" and "ApplyFlowPropertyParam" have been applied)
                    var fragmentDependencies = ApplyFlowPropertyParam(fragmentID, scenarioId).Where(q => q.ParentFragmentFlowID == currentFlow).AsEnumerable();

                    //get the fragment associated with the subFragmentId - actually don't think we need this.  
                    //We can just pass the subfragmentid and the scenarioId and call the parent 'Traverse' method
                    //var subFragment = _fragmentService.Query().Filter(q => q.FragmentID == subFragmentId).Get().AsQueryable();

                    //change the value of fragmentId to the value of subFragementId and traverse the subfragment
                    fragmentID = Convert.ToInt32(subFragmentId);
                    Traverse(fragmentID, scenarioId);

                    if (subFragmentId != null)
                    {
                        MapDependencies(fragmentDependencies, fragmentID, scenarioId);
                    }

                    //update each flow quantity with the fragmentDependency quantity
                    var updateFlows = ApplyFlowPropertyParam(fragmentID, scenarioId).Join(fragmentDependencies, p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc }).AsQueryable();
                    foreach (var item in updateFlows)
                    {
                        item.p.Quantity = item.pc.Quantity;
                    }

                    //recurse again
                    foreach (var item in fragmentDependencies)
                    {
                        NodeRecurse(scenarioId, item.FragmentFlowID, activity, fragmentID);
                    }

                    break;
                case 3: //InputOutput
                    // nothing to do - cache value specifies input/output quantity
                    break;

                case 4: //background
                    //nothing to do - background fragments have already been traversed
                    break;
            }

        }

        public IEnumerable<FlowPropertyParamModel> MapDependencies(IEnumerable<FlowPropertyParamModel> dep, int fragmentID, int scenarioId = 1)
        {
            var nodeCache = _nodeCacheService.Query().Get().AsEnumerable();

            //Get distinct flows, their direction ids a
            var myIO = ApplyFlowPropertyParam(fragmentID, scenarioId)
                .Join(nodeCache, p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                .Where(q => q.p.NodeTypeID == 3)
                .Where(q => q.pc.ScenarioID == scenarioId)
                .Select(t => new {t.p.FlowID, t.p.DirectionID, t.pc.NodeWeight})
             .GroupBy(t => new 
             {
                 t.FlowID, 
                 t.DirectionID,
                 t.NodeWeight
             })
             .Select(group => new  { 
                       FlowID = group.Key.FlowID, 
                       DirectionID = group.Key.DirectionID,
                       IOSUM = group.Sum(a => a.NodeWeight)
             });

                  //.GroupBy(p => new
                  // {
                  //     FlowID = p.p.FlowID,
                  //     DirectionID = p.p.DirectionID,
                  //     NodeWeight = p.pc.NodeWeight

                  // })
                  // .Select(p => new
                  // {
                  //     FlowID = p.Key.FlowID,
                  //     DirectionID = p.Key.DirectionID,
                  //     IOSUM = p.Key.NodeWeight

                  // }).AsQueryable();

            var newDep = dep.AsEnumerable()
                .Join(myIO, p => p.FlowID, pc => pc.FlowID, (p, pc) => new { p, pc })
                .Where(q => q.p.DirectionID == q.pc.DirectionID)
                 .Select(p => new FlowPropertyParamModel
                   {
                       Quantity = p.p.Quantity,
                       NodeWeight = p.pc.IOSUM

                   }).AsEnumerable();

            foreach (var item in newDep)
            {
                item.Quantity = item.NodeWeight;
            }

            return newDep;
        }
    }
}
