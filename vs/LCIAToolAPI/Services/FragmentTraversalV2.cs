using LcaDataModel;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using Entities.Models;

namespace Services
{

    //Version 2 of fragment traversal - rewritten to reflect the pseudocode dated Mon Jul 28 00:32:01 -0700 2014
    public class FragmentTraversalV2 : IFragmentTraversalV2
    {
        //Trying to inject a generic service to avoid having a service for each model class - reduce bloat in architecture.
        //[Inject]
        //private readonly IService<Flow> _flowService;
        //[Inject]
        //private readonly IService<FragmentFlow> _fragmentFlowService;
        //public FragmentTraversalV2(IService<Flow> flowService, IService<FragmentFlow> fragmentFlowService)
        //{ 
        //    _flowService = flowService;
        //    _fragmentFlowService = fragmentFlowService;
        //}


        [Inject]
        private readonly IFlowService _flowService;
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly INodeCacheService _nodeCacheService;
        [Inject]
        private readonly IFragmentNodeProcessService _fragmentNodeProcessService;
        [Inject]
        private readonly IProcessFlowService _processFlowService;
        [Inject]
        private readonly IFragmentNodeFragmentService _fragmentNodeFragmentService;
        [Inject]
        private readonly IFlowFlowPropertyService _flowFlowPropertyService;
        [Inject]
        private readonly IDependencyParamService _dependencyParamService;
        [Inject]
        private readonly IScenarioParamService _scenarioParamService;
        [Inject]
        private readonly IFlowPropertyParamService _flowPropertyParamService;
        [Inject]
        private readonly IFragmentService _fragmentService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;


        public FragmentTraversalV2(IFlowService flowService,
            IFragmentFlowService fragmentFlowService,
            INodeCacheService nodeCacheService,
            IFragmentNodeProcessService fragmentNodeProcessService,
            IProcessFlowService processFlowService,
            IFragmentNodeFragmentService fragmentNodeFragmentService,
            IFlowFlowPropertyService flowFlowPropertyService,
            IDependencyParamService dependencyParamService,
            IScenarioParamService scenarioParamService,
            IFlowPropertyParamService flowPropertyParamService,
            IUnitOfWork unitOfWork,
            IFragmentService fragmentService)
        {
            _flowService = flowService;
            _fragmentFlowService = fragmentFlowService;
            _nodeCacheService = nodeCacheService;
            _fragmentNodeProcessService = fragmentNodeProcessService;
            _processFlowService = processFlowService;
            _fragmentNodeFragmentService = fragmentNodeFragmentService;
            _flowFlowPropertyService = flowFlowPropertyService;
            _dependencyParamService = dependencyParamService;
            _scenarioParamService = scenarioParamService;
            _flowPropertyParamService = flowPropertyParamService;
            _fragmentService = fragmentService;
            _unitOfWork = unitOfWork;
        }



        public bool Traverse(int? fragmentId = 11, int scenarioId = 1)
        {
            int refFlow = _fragmentFlowService.Query()
                .Filter(q => q.FragmentID == fragmentId)
                .Get()
                .Where(q => q.ParentFragmentFlowID == null)
                .FirstOrDefault()
                .FragmentFlowID;

            float? activity = 1;

            var chk = _nodeCacheService.Query()
                .Filter(q => q.FragmentFlowID == refFlow)
                .GetPage()
                .Where(q => q.ScenarioID == scenarioId).Count();



            if (chk == 0)
            {
                NodeRecurse(refFlow, scenarioId, activity);
                return true;
            }
            else
            {
                return false;
            }


        }

        public void NodeRecurse(int fragmentFlowId, int scenarioId, double? flowMagnitude)
        {
            var theFlow = _fragmentFlowService.Query()
                .Filter(q => q.FragmentFlowID == fragmentFlowId)
                .Get().AsEnumerable<FragmentFlow>();

            var nodeFlows = GetScenarioProductFlows(theFlow, scenarioId).ToList();

            int? termFlowId = GetTermFlow(theFlow);

            int? theFlowDirection = Convert.ToInt32(theFlow.Select(x => x.DirectionID).FirstOrDefault());

            double? nodeConv = null;

            switch (theFlowDirection)
            {
                case 1:
                    theFlowDirection = 2;
                    break;
                case 2:
                    theFlowDirection = 1;
                    break;
            }

            var inFlow = nodeFlows
                    .Where(x => x.FlowID == termFlowId)
                    .Where(x => x.DirectionID == theFlowDirection)

                         .Select(b => new NodeFlowModel
                    {
                        FlowID = b.FlowID,
                        DirectionID = b.DirectionID,
                        FlowMagnitude = b.FlowMagnitude,
                        Result = b.Result
                    }).ToList();



            int? theFlowId = theFlow.Select(x => x.FlowID).FirstOrDefault();


            //set theFlowId to termFlowId in cases where FlowID is null (eg records with a null for ParentFragmentFlowID)


            if (theFlowId == termFlowId || theFlowId == null)
            {
                nodeConv = 1;
            }
            else
            {
                var termFlow = _flowService.Query().Filter(x => x.FlowID == termFlowId).Get().FirstOrDefault();

                var conv = _flowFlowPropertyService.Query().Get()
                    .GroupJoin(_flowPropertyParamService.Query().Get() // Target table
                , dp => dp.FlowFlowPropertyID
                , sp => sp.FlowFlowPropertyID
                , (dp, sp) => new { dependencyParams = dp, scenarioParams = sp })
                .SelectMany(s => s.scenarioParams.DefaultIfEmpty()
                , (s, scenarioparams) => new
                {

                    ParamID = scenarioparams == null ? 0 : scenarioparams.ParamID,
                    FlowPropertyID = s.dependencyParams.FlowPropertyID,
                    FlowID = s.dependencyParams.FlowID,
                    MeanValue = s.dependencyParams.MeanValue,
                    Value = scenarioparams == null ? 0 : scenarioparams.Value,
                })
                    //we will join with scenario later - no point doing an expensive left outer join just to make it work
                    //.Join(_scenarioParamService.Query().Get(), p => p.ParamID, pc => pc.ParamID, (p, pc) => new { p, pc })
                    .Where(x => x.FlowID == theFlowId)
                    .Where(x => x.FlowPropertyID == termFlow.ReferenceFlowProperty)
                    //.Where(x => x.pc.ScenarioID == scenarioId)
                    .Select(b => new
                    {
                        Default = b.MeanValue,
                        Subs = b.Value == null ? 0 : b.Value,
                        //Result = b.p.pc.Value == null ? b.p.p.MeanValue : b.p.pc.Value
                    });




                double? convDefault = conv.Select(x => x.Default).FirstOrDefault();
                double? convSubs = conv.Select(x => x.Subs).FirstOrDefault();

                //nodeConv = conv.Select(x => x.Result).FirstOrDefault();

                if (convSubs != 0)
                {
                    nodeConv = convSubs;
                }
                else
                {
                    nodeConv = convDefault;
                }
            }


            double? nodeScale = 1 / inFlow.Select(x => x.Result).FirstOrDefault();
            double? nodeWeight = flowMagnitude * nodeConv * nodeScale;


            var nodeCache = new NodeCache
            {
                FragmentFlowID = fragmentFlowId,
                ScenarioID = scenarioId,
                FlowMagnitude = flowMagnitude,
                NodeWeight = nodeWeight
            };
            _nodeCacheService.InsertGraph(nodeCache);
            _unitOfWork.Save();

            var outFlows = _fragmentFlowService.Query().Get()
                .Where(x => x.ParentFragmentFlowID == fragmentFlowId)
                .GroupJoin(nodeFlows // Target table
, ff => ff.FlowID
, nf => nf.FlowID
, (ff, nf) => new { fragmentflows = ff, nodeFlows = nf })
.SelectMany(s => s.nodeFlows.DefaultIfEmpty()
, (s, nodeflows) => new
{
    FragmentFlowID = s.fragmentflows.FragmentFlowID,
    FlowID = s.fragmentflows.FlowID,
    FFDirectionID = s.fragmentflows.DirectionID,
    NFDirectionID = nodeflows.DirectionID,
    Result = nodeflows.Result,
    ParentFragmentFlowID = s.fragmentflows.ParentFragmentFlowID
})

.GroupJoin(_dependencyParamService.Query().Get() // Target table
                , ff => ff.FragmentFlowID
                , dp => dp.FragmentFlowID
                , (ff, dp) => new { fragmentflows = ff, dependencyparams = dp })
                .SelectMany(s => s.dependencyparams.DefaultIfEmpty()
                , (s, dependencyparams) => new
                {
                    FragmentFlowID = s.fragmentflows.FragmentFlowID,
                    FlowID = s.fragmentflows.FlowID,
                    FFDirectionID = s.fragmentflows.FFDirectionID,
                    NFDirectionID = s.fragmentflows.NFDirectionID,
                    Result = s.fragmentflows.Result,
                    ParamID = dependencyparams == null ? -1 : dependencyparams.ParamID,
                    ParamValue = dependencyparams == null ? 0 : dependencyparams.Value,
                    ParentFragmentFlowID = s.fragmentflows.ParentFragmentFlowID
                })

.GroupJoin(_scenarioParamService.Query().Get() // Target table
                , dp => dp.ParamID
                , sp => sp.ParamID
                , (dp, sp) => new { dependencyParams = dp, scenarioParams = sp })
                .SelectMany(s => s.scenarioParams.DefaultIfEmpty()
                , (s, scenarioparams) => new TestOutFlowModel
                {
                    ScenarioID = scenarioparams == null ? 1 : scenarioparams.ScenarioID,
                    FragmentFlowID = s.dependencyParams.FragmentFlowID,
                    FlowID = s.dependencyParams.FlowID,
                    FFDirectionID = s.dependencyParams.FFDirectionID,
                    NFDirectionID = s.dependencyParams.NFDirectionID,
                    Result = s.dependencyParams.Result,
                    ParamID = s.dependencyParams.ParamID,
                    ParamValue = s.dependencyParams.ParamValue,
                    ParentFragmentFlowID = s.dependencyParams.ParentFragmentFlowID
                }).ToList()


.Where(x => x.FFDirectionID == x.NFDirectionID);
            //.Where(x => x.ScenarioID == scenarioId)


            if (outFlows != null)
            {
                foreach (var item in outFlows)
                {

                    try
                    {
                        double outflowMagnitude = Convert.ToDouble(nodeWeight * item.Result);
                        int outflowScenarioId = Convert.ToInt32(item.ScenarioID);
                        int outflowFragmentFlowID = Convert.ToInt32(item.FragmentFlowID);

                        if (item.ParamValue != null)
                        {
                            item.Result = item.ParamValue;
                        }

                        NodeRecurse(outflowFragmentFlowID, outflowScenarioId, outflowMagnitude);
                    }
                    catch (ArgumentNullException e)
                    {
                        throw new ArgumentNullException("This value cannot be null.", e);
                    }



                }
            }












        }

        public IEnumerable<NodeFlowModel> GetScenarioProductFlows(IEnumerable<FragmentFlow> theFragmentFlow, int scenarioId)
        {
            int? nodeTypeID = theFragmentFlow.Select(x => x.NodeTypeID).FirstOrDefault();

            //NodeFlowModel nodeFlowModel = new NodeFlowModel();

            IEnumerable<NodeFlowModel> nodeFlowModel = null;

            switch (nodeTypeID)
            {
                case 1: //process

                    int? processId = theFragmentFlow
                        .Join(_fragmentNodeProcessService.Query().Get(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                        .FirstOrDefault()
                        .pc.ProcessID;
                    //also needs left outer join on substitution but we don't have those tables yet.
                    //add this later when the substitution tables have been added
                    //if Subs is not null
                    //    process_id = Subs;
                    //else
                    //    process_id = Default;

                    nodeFlowModel = _processFlowService.Query().Get()
                        .Join(_flowService.Query().Get(), p => p.FlowID, pc => pc.FlowID, (p, pc) => new { p, pc })
                        .Where(x => x.p.ProcessID == processId)
                        .Where(x => x.pc.FlowTypeID == 1)
                          .Select(nfm => new NodeFlowModel
                          {
                              FlowID = nfm.p.FlowID,
                              DirectionID = nfm.p.DirectionID,
                              Result = nfm.p.Result
                          });



                    break;
                case 2: //fragment

                    //get the sub fragment
                    int? subFragmentId = theFragmentFlow
                        .Join(_fragmentNodeFragmentService.Query().Get(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                        .FirstOrDefault()
                        .pc.SubFragmentID;
                    //also needs left outer join on substitution but we don't have those tables yet.
                    //add this later when the substitution tables have been added
                    //if Subs is not null
                    //fragment_id = Subs;
                    //else
                    //fragment_id = Default;

                    int? fragmentId = theFragmentFlow
                       .FirstOrDefault()
                       .FragmentID;



                    Traverse(subFragmentId, scenarioId);


                    var fragmentNodeFlows = _fragmentFlowService.Query().Get()
                         .Where(x => x.FragmentFlowID == theFragmentFlow.Select(y => y.FragmentFlowID).FirstOrDefault())
                         //.Where(x => x.FragmentID == fragmentId)
                        .Join(_fragmentNodeFragmentService.Query().Get(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                       
                        .Join(_fragmentService.Query().Get(), p => p.pc.SubFragmentID, pc => pc.FragmentID, (p, pc) => new { p, pc })
                         .Select(a => new NodeFlowModel
                    {
                        FragmentFlowID = a.pc.ReferenceFragmentFlowID,
                        FlowID = a.p.pc.FlowID,
                        DirectionID = a.p.p.DirectionID,
                        FragmentID = a.p.p.FragmentID,
                        NodeTypeID = a.p.p.NodeTypeID
                    })
                    
                    .Union(_fragmentFlowService.Query().Get().Select(
                    b => new NodeFlowModel
                    {
                        FragmentFlowID = b.FragmentFlowID,
                        FlowID = b.FlowID,
                        DirectionID = b.DirectionID, 
                        FragmentID = b.FragmentID,
                        NodeTypeID = b.NodeTypeID
                    })
                    .Where(x => x.FragmentID == subFragmentId && x.NodeTypeID == 3));
                    

                    // next we need to modify the table to fix the reference flow (FlowID = null) and
                    // make it appear like an InputOutput flow
                    var updateNodeFlows = fragmentNodeFlows.Where(x => x.FlowID == 0).AsEnumerable();
                    foreach (var item in updateNodeFlows)
                    {
                        switch (item.DirectionID)
                        {
                            case 1:
                                item.DirectionID = 2;
                                break;
                            case 2:
                                item.DirectionID = 1;
                                break;
                        }

                        item.FlowID = item.RefFlowID;
                    }

                    nodeFlowModel = fragmentNodeFlows
                        .Join(_nodeCacheService.Query().Get(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                        .Where(x => x.pc.ScenarioID == scenarioId)
             .GroupBy(t => new
             {
                 t.p.FlowID,
                 t.p.DirectionID,
                 t.pc.FlowMagnitude,
                 t.pc.FragmentFlowID
             })
             .Select(group => new NodeFlowModel
             {
                 FlowID = group.Key.FlowID,
                 DirectionID = group.Key.DirectionID,
                 Result = group.Sum(a => a.pc.FlowMagnitude),
                 FragmentFlowID = group.Key.FragmentFlowID
             });

                    break;
                case 3:
                case 4: //InputOutput and Background

                    nodeFlowModel = theFragmentFlow.Select(t => new NodeFlowModel
                    {
                        FlowID = t.FlowID,
                        DirectionID = t.DirectionID,
                        Result = 1,
                        FragmentFlowID = t.FragmentFlowID
                    });
                    break;
            }

            return nodeFlowModel;

        }

        public int? GetTermFlow(IEnumerable<FragmentFlow> theFragmentFlow)
        {
            int? nodeTypeID = theFragmentFlow.Select(x => x.NodeTypeID).FirstOrDefault();

            int? termFlowId = 0;
            switch (nodeTypeID)
            {
                case 3:
                case 4:
                    termFlowId = theFragmentFlow.Select(x => x.FlowID).FirstOrDefault();
                    break;

                case 1:
                    var termFlowProcess = theFragmentFlow.Join(_fragmentNodeProcessService.Query().Get(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                    .Select(x => x.pc.FlowID).FirstOrDefault();
                    termFlowId = Convert.ToInt32(termFlowProcess.Value);
                    break;

                case 2:
                    var termFlowFragment = theFragmentFlow.Join(_fragmentNodeFragmentService.Query().Get(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                    .Select(x => x.pc.FlowID).FirstOrDefault();
                    termFlowId = Convert.ToInt32(termFlowFragment.Value);
                    break;
            }

            return termFlowId;

        }
    }
}
