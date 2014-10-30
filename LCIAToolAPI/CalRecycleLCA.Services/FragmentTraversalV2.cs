﻿using LcaDataModel;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using Entities.Models;

namespace CalRecycleLCA.Services
{

    //Version 2 of fragment traversal - rewritten to reflect the pseudocode dated Mon Jul 28 00:32:01 -0700 2014
    public class FragmentTraversalV2 : IFragmentTraversalV2
    {
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
        private readonly IParamService _paramService;
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
            IFlowPropertyParamService flowPropertyParamService,
            IFragmentService fragmentService,
            IParamService paramService,
            IUnitOfWork unitOfWork)
        {
            _flowService = flowService;
            _fragmentFlowService = fragmentFlowService;
            _nodeCacheService = nodeCacheService;
            _fragmentNodeProcessService = fragmentNodeProcessService;
            _processFlowService = processFlowService;
            _fragmentNodeFragmentService = fragmentNodeFragmentService;
            _flowFlowPropertyService = flowFlowPropertyService;
            _dependencyParamService = dependencyParamService;
            _paramService = paramService;
            _flowPropertyParamService = flowPropertyParamService;
            _fragmentService = fragmentService;
            _unitOfWork = unitOfWork;
        }



        public bool Traverse(int? fragmentId = 0, int scenarioId = 0)
        {
            int refFlow = _fragmentFlowService
                .Query(q => q.FragmentID == fragmentId && q.ParentFragmentFlowID == null)
                .Select()
                .FirstOrDefault()
                .FragmentFlowID;

            float? activity = 1;

            var chk = _nodeCacheService
                .Query(q => q.FragmentFlowID == refFlow && q.ScenarioID == scenarioId)
                .Select()
                .Count();


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
            var theFlow = _fragmentFlowService
                .Query(q => q.FragmentFlowID == fragmentFlowId)
                .Select()
                .AsEnumerable<FragmentFlow>();

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
                var termFlow = _flowService.Query(x => x.FlowID == termFlowId).Select().FirstOrDefault();
                
                var conv = _flowFlowPropertyService.Queryable()
                    .GroupJoin(_flowPropertyParamService.Queryable() // Target table
                , ffp => ffp.FlowFlowPropertyID
                , fpp => fpp.FlowFlowPropertyID
                , (ffp, fpp) => new { flowFlowProperties = ffp, flowPropertyParams = fpp })
                .SelectMany(s => s.flowPropertyParams.DefaultIfEmpty()
                , (s, flowPropertyParams) => new
                {

                    ParamID = flowPropertyParams == null ? 0 : flowPropertyParams.ParamID,
                    FlowPropertyID = s.flowFlowProperties.FlowPropertyID,
                    FlowID = s.flowFlowProperties.FlowID,
                    MeanValue = s.flowFlowProperties.MeanValue,
                    Value = flowPropertyParams == null ? 0 : flowPropertyParams.Value,
                })
                .GroupJoin(_paramService.Queryable() // Target table
                , fpp => fpp.ParamID
                , p => p.ParamID
                , (fpp, p) => new { flowPropertyParams = fpp, parameters = p })
                .SelectMany(s => s.parameters.DefaultIfEmpty()
                , (s, parameters) => new
                {

                    ParamID = parameters == null ? 0 : parameters.ParamID,
                    FlowPropertyID = s.flowPropertyParams.FlowPropertyID,
                    FlowID = s.flowPropertyParams.FlowID,
                    MeanValue = s.flowPropertyParams.MeanValue,
                    Value = s.flowPropertyParams.Value,
                    ScenarioID = parameters == null ? 0 : parameters.ScenarioID
                })
                    .Select(b => new
                    {
                        Default = b.MeanValue,
                        Subs = b.Value == null ? 0 : b.Value
                    });




                double? convDefault = conv.Select(x => x.Default).FirstOrDefault();
                double? convSubs = conv.Select(x => x.Subs).FirstOrDefault();

                if (convSubs != 0)
                {
                    nodeConv = convSubs;
                }
                else
                {
                    nodeConv = convDefault;
                }
            }

            double? nodeScale;
            if (inFlow.Select(x => x.Result).FirstOrDefault() == 0)
            {
                throw new ArgumentException("The inflow result cannot be 0");
            }
            else
            {
                nodeScale = 1 / inFlow.Select(x => x.Result).FirstOrDefault();
            }

            
            double? nodeWeight = flowMagnitude * nodeConv * nodeScale;

            //do not save to NodeCache and abandon recursion if nodeweight == 0
            if (nodeWeight != 0)
            {


                var outFlows = _fragmentFlowService.Queryable()
                    .Where(x => x.ParentFragmentFlowID == fragmentFlowId).ToList()

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
        NFDirectionID = nodeflows == null ? 0 : nodeflows.DirectionID,
        Result = nodeflows == null ? 0 : nodeflows.Result,
        ParentFragmentFlowID = s.fragmentflows.ParentFragmentFlowID
    })

    .GroupJoin(_dependencyParamService.Queryable().ToList() // Target table
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

    .GroupJoin(_paramService.Queryable()
   .Where(x => x.ScenarioID == scenarioId).ToList()
                    , dp => dp.ParamID
                    , sp => sp.ParamID
                    , (dp, sp) => new { dependencyParams = dp, scenarioParams = sp })
                    .SelectMany(s => s.scenarioParams.DefaultIfEmpty()
                    , (s, scenarioparams) => new OutFlowModel
                    {
                        ScenarioID = scenarioparams == null ? scenarioId : scenarioparams.ScenarioID,
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


                if (outFlows != null)
                {
                    foreach (var item in outFlows)
                    {
                      
                        if (item.Result == null)
                        {
                            throw new ArgumentNullException("This value cannot be null.");
                        }
                        else
                        {
                            if (item.ParamValue != 0  && scenarioId != 0)
                            {
                                item.Result = item.ParamValue;
                            }
                            double outflowMagnitude = Convert.ToDouble(nodeWeight * item.Result);
                            int outflowFragmentFlowID = Convert.ToInt32(item.FragmentFlowID);
                            NodeRecurse(outflowFragmentFlowID, scenarioId, outflowMagnitude);
                        }
                    }
                }

                var nodeCache = new NodeCache
                {
                    FragmentFlowID = fragmentFlowId,
                    ScenarioID = scenarioId,
                    FlowMagnitude = flowMagnitude,
                    NodeWeight = nodeWeight
                };

                nodeCache.ObjectState = ObjectState.Added;
                _nodeCacheService.InsertOrUpdateGraph(nodeCache);
                _unitOfWork.SaveChanges();
            }

        }

        public IEnumerable<NodeFlowModel> GetScenarioProductFlows(IEnumerable<FragmentFlow> theFragmentFlow, int scenarioId)
        {
            int nodeTypeID = Convert.ToInt32(theFragmentFlow.Select(x => x.NodeTypeID).FirstOrDefault());
            int theFragmentFlowId = theFragmentFlow.Select(x => x.FragmentFlowID).FirstOrDefault();
            int? fragmentId = theFragmentFlow.FirstOrDefault().FragmentID;

            IEnumerable<NodeFlowModel> nodeFlowModel = null;

            switch (nodeTypeID)
            {
                case 1: //process

                    //get the process id
                    int? processId = _fragmentNodeProcessService.GetFragmentNodeProcessId(theFragmentFlowId).ProcessID;

                    //old query for processId before I implemented extension method to query for ProcessID, SubFragmentID and TermFlowID
                    //int? processId = theFragmentFlow
                    //    .Join(_fragmentNodeProcessService.Queryable(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                    //    .FirstOrDefault()
                    //    .pc.ProcessID;
                    ////also needs left outer join on substitution but we don't have those tables yet.
                    ////add this later when the substitution tables have been added
                    ////if Subs is not null
                    ////    process_id = Subs;
                    ////else
                    ////    process_id = Default;

                    nodeFlowModel = _processFlowService.Queryable()
                        .Join(_flowService.Queryable(), p => p.FlowID, pc => pc.FlowID, (p, pc) => new { p, pc })
                        .Where(x => x.p.ProcessID == processId)
                        .Where(x => x.pc.FlowTypeID == 1)
                          .Select(nfm => new NodeFlowModel
                          {
                              FlowID = nfm.p.FlowID,
                              DirectionID = nfm.p.DirectionID,
                              Result = nfm.p.Result
                          });



                    break;
                case 3:
                case 4: //InputOutput and Background

                    var updatedirection = theFragmentFlow.Select(x => x.DirectionID).FirstOrDefault();

                    switch (updatedirection)
                    {
                        case 1:
                            updatedirection = 2;
                            break;
                        case 2:
                            updatedirection = 1;
                            break;
                    }

                    nodeFlowModel = theFragmentFlow.Select(t => new NodeFlowModel
                    {
                        FlowID = t.FlowID,
                        DirectionID = updatedirection,
                        Result = 1
                    });
                    break;
                case 2: //fragment

                    //get the sub fragment id
                    int? subFragmentId = _fragmentNodeFragmentService.GetFragmentNodeSubFragmentId(theFragmentFlowId).SubFragmentID;

                    ////old query for subFragmentId before I implemented extension method to query for ProcessID, SubFragmentID and TermFlowID
                    //int? subFragmentId = theFragmentFlow
                    //    .Join(_fragmentNodeFragmentService.Queryable(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                    //    .FirstOrDefault()
                    //    .pc.SubFragmentID;
                    ////also needs left outer join on substitution but we don't have those tables yet.
                    ////add this later when the substitution tables have been added
                    ////if Subs is not null
                    ////fragment_id = Subs;
                    ////else
                    ////fragment_id = Default;




                    Traverse(subFragmentId, scenarioId);


                    var fragmentNodeFlows = _fragmentFlowService.Queryable().ToList()
                         .Where(x => x.FragmentFlowID == theFragmentFlow.Select(y => y.FragmentFlowID).FirstOrDefault())
                        //.Where(x => x.FragmentID == fragmentId)
                        .Join(_fragmentNodeFragmentService.Queryable(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })

                        .Join(_fragmentService.Queryable(), p => p.pc.SubFragmentID, pc => pc.FragmentID, (p, pc) => new { p, pc })
                         .Select(a => new NodeFlowModel
                    {
                        FragmentFlowID = a.pc.ReferenceFragmentFlowID,
                        FlowID = a.p.pc.FlowID,
                        DirectionID = a.p.p.DirectionID,
                        FragmentID = a.p.p.FragmentID,
                        NodeTypeID = a.p.p.NodeTypeID
                    })
                    .Union(_fragmentFlowService.Queryable().Select(
                    b => new NodeFlowModel
                    {
                        FragmentFlowID = b.FragmentFlowID,
                        FlowID = b.FlowID,
                        DirectionID = b.DirectionID,
                        FragmentID = b.FragmentID,
                        NodeTypeID = b.NodeTypeID
                    })
                    .Where(x => x.FragmentID == subFragmentId && x.NodeTypeID == 3))
                    .ToList();


                   

                    // next we need to modify the table to fix the reference flow (FlowID = null) and
                    // make it appear like an InputOutput flow
                    fragmentNodeFlows = fragmentNodeFlows.ToList();
                    foreach (var item in fragmentNodeFlows)
                    {
                        if (item.FragmentID == fragmentId)
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
                        }
                    }

                    nodeFlowModel = fragmentNodeFlows
                        .Join(_nodeCacheService.Queryable(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
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

            }

            return nodeFlowModel;

        }

        public int? GetTermFlow(IEnumerable<FragmentFlow> theFragmentFlow)
        {
            int nodeTypeID = Convert.ToInt32(theFragmentFlow.Select(x => x.NodeTypeID).FirstOrDefault());
            int theFragmentFlowId = theFragmentFlow.Select(x => x.FragmentFlowID).FirstOrDefault();

            int? termFlowId = 0;
            switch (nodeTypeID)
            {
                case 3:
                case 4:
                    termFlowId = theFragmentFlow.Select(x => x.FlowID).FirstOrDefault();
                    break;

                case 1:
                    termFlowId = _fragmentNodeProcessService.GetFragmentNodeProcessId(theFragmentFlowId).TermFlowID;

                    ////old query for termFlowId before I implemented extension method to query for ProcessID, SubFragmentID and TermFlowID
                    //var termFlowProcess = theFragmentFlow.Join(_fragmentNodeProcessService.Queryable(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                    //.Select(x => x.pc.FlowID).FirstOrDefault();
                    //termFlowId = Convert.ToInt32(termFlowProcess.Value);
                    break;

                case 2:
                    termFlowId = _fragmentNodeFragmentService.GetFragmentNodeSubFragmentId(theFragmentFlowId).TermFlowID;

                    //old query for termFlowId before I implemented extension method to query for ProcessID, SubFragmentID and TermFlowID
                    //var termFlowFragment = theFragmentFlow.Join(_fragmentNodeFragmentService.Queryable(), p => p.FragmentFlowID, pc => pc.FragmentFlowID, (p, pc) => new { p, pc })
                    //.Select(x => x.pc.FlowID).FirstOrDefault();
                    //termFlowId = Convert.ToInt32(termFlowFragment.Value);
                    break;
            }

            return termFlowId;

        }
    }
}