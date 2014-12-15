using LcaDataModel;
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
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly INodeCacheService _nodeCacheService;
        [Inject]
        private readonly IProcessFlowService _processFlowService;
        [Inject]
        private readonly IFlowFlowPropertyService _flowFlowPropertyService;
        [Inject]
        private readonly IDependencyParamService _dependencyParamService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;

        private List<NodeCache> nodeCaches;


        public FragmentTraversalV2(
            IFragmentFlowService fragmentFlowService,
            INodeCacheService nodeCacheService,
            IProcessFlowService processFlowService,
            IFlowFlowPropertyService flowFlowPropertyService,
            IDependencyParamService dependencyParamService,
            IUnitOfWork unitOfWork)
        {
            _fragmentFlowService = fragmentFlowService;
            _nodeCacheService = nodeCacheService;
            _processFlowService = processFlowService;
            _flowFlowPropertyService = flowFlowPropertyService;
            _dependencyParamService = dependencyParamService;
            _unitOfWork = unitOfWork;
        }



        /// <summary>
        /// Traverse a fragment by recursively following fragmentflow links.
        /// 
        /// This function needs to be updated to use fragment In-flows.
        /// 
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public bool Traverse(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            nodeCaches = new List<NodeCache>();

            var fragmentFlows = _fragmentFlowService.GetFlowsByFragment(fragmentId);
            var dependencyParams = _dependencyParamService.Query(dp => dp.Param.ScenarioID == scenarioId).Select().ToList();

            float activity = 1;

            int refFlow = fragmentFlows.Where(k => k.ParentFragmentFlow == null).First().FragmentFlowID;

            var chk = _nodeCacheService
                .Query(q => q.FragmentFlowID == refFlow && q.ScenarioID == scenarioId)
                .Select()
                .Count();

            if (chk == 0)
            {
                _unitOfWork.SetAutoDetectChanges(false);
                NodeRecurse(fragmentFlows, dependencyParams, refFlow, scenarioId, activity);

                foreach (var nodeCache in nodeCaches)
                {
                    nodeCache.ObjectState = ObjectState.Added;
                }
                _nodeCacheService.InsertGraphRange(nodeCaches);
                _unitOfWork.SaveChanges();
                _unitOfWork.SetAutoDetectChanges(true);
                return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// Traverse a fragment by recursively following fragmentflow links.  nodes of type
        /// 3 or 4 are terminal cases; nodes of type 1 or 2 are recursive.
        /// It is a data integrity requirement that a given process or subfragment has the same 
        /// dependencies as the fragment node in which it is instantiated.
        /// </summary>
        /// <param name="fragmentFlowId">FragmentFlow link being traversed</param>
        /// <param name="scenarioId">scenario for current traversal</param>
        /// <param name="flowMagnitude">magnitude of current link exiting parent node.</param>
        public void NodeRecurse(IEnumerable<FragmentFlow> ff, 
                                IEnumerable<DependencyParam> ff_param,
                                int fragmentFlowId, int scenarioId, double flowMagnitude)
        {
            var theFragmentFlow = ff.Where(k => k.FragmentFlowID == fragmentFlowId).First();
            
            FragmentNodeResource term = _fragmentFlowService.Terminate(theFragmentFlow,scenarioId); // don't bother to resolve background

            // first, calculate node weight
            double? flow_conv = _flowFlowPropertyService.FlowConv(theFragmentFlow.FlowID, term.TermFlowID, scenarioId);
            // and incoming exchange
            // note this is not subject to parameter adjustment
            double flow_exch;
            var outFlows = GetScenarioProductFlows(term, theFragmentFlow.DirectionID, out flow_exch);
            
            if (flow_conv == null)
            {
                throw new ArgumentNullException("Flow conversion was not found and cannot be null");
            }

            if (flow_exch == 0)
            {
                throw new ArgumentException("The inflow result cannot be 0");
            }

            double nodeWeight = flowMagnitude * (double)flow_conv / flow_exch;

            // do not cache if nodeweight == 0 
            if (nodeWeight != 0)
            {
                // begin recursion by finding all FragmentFlows with the current parent
                IEnumerable<FragmentFlow> outLinks = ff
                    .Where(k => k.ParentFragmentFlowID == fragmentFlowId).ToList();

                IEnumerable<int> outFFids = outLinks.Select(a => a.FragmentFlowID);

                // Background: when designing fragments in Matlab, I allowed the user to exclude 
                // process flows that were not desired to be included in the fragment, rendering 
                // them in effect as cutoff flows.
                // Problem: during traversal, these flows will still show up in outFlows, causing 
                // reconciliation error.
                // interim solution: relax the reconciliation requirement as long as there are
                // more outFlows than outLinks. (i.e. allow unlinked outFlows to be cutoff)
                // Background: when designing fragments in Matlab, I allowed the user to exclude 
                // process flows that were not desired to be included in the fragment, rendering 
                // them in effect as cutoff flows.
                // Problem: during traversal, these flows will still show up in outFlows, causing 
                // reconciliation error.
                // interim solution: relax the reconciliation requirement as long as there are
                // more outFlows than outLinks. (i.e. allow unlinked outFlows to be treated as cutoffs)
                if (outLinks.Count() > outFlows.Count()) // TODO -- this should be !=
                {
                    throw new ArgumentException("OutFlows and OutLinks don't reconcile!");
                }

                // abandon recursion if no recursive steps-- but foreach should just not run if outLinks is empty
                foreach (var item in outLinks)
                {
                    var this_param = ff_param.Where(dp => dp.FragmentFlowID == item.FragmentFlowID).FirstOrDefault();
                    double resultVal = (double)outFlows
                        .Where(o => o.FlowID == item.FlowID && o.DirectionID == item.DirectionID)
                        .First().Result; // First() should generate exception if no match is found

                    if (this_param != null)
                        resultVal = this_param.Value;

                    NodeRecurse(ff, ff_param, item.FragmentFlowID, scenarioId, nodeWeight * resultVal);
                }
            }

            //var nodeCache = new NodeCache
            //{
            //    FragmentFlowID = fragmentFlowId,
            //    ScenarioID = scenarioId,
            //    FlowMagnitude = flowMagnitude,
            //    NodeWeight = nodeWeight
            //};
            
            //nodeCache.ObjectState = ObjectState.Added;
            //_nodeCacheService.InsertOrUpdateGraph(nodeCache);

            
            nodeCaches.Add(new NodeCache()
            {
                FragmentFlowID = fragmentFlowId,
                ScenarioID = scenarioId,
                FlowMagnitude = flowMagnitude,
                NodeWeight = nodeWeight
            });

            
            
        }

                /// <summary>
        /// Determine dependency flows required by the fragmentflow termination being supplied.
        /// This function assumes that all flow+direction combinations are distinct for processes and fragments.
        /// (for fragments, node outflows are grouped by flow+direction)
        /// </summary>
        /// <param name="term">FragmentNodeResource corresponding to the terminus of the current FragmentFlow</param>
        /// <param name="ex_directionId">Direction of the physical flow with respect to the *parent*</param>
        /// <param name="flow_exch">out param set equal to the inflow's exchange</param>
        /// <returns></returns>
        public IEnumerable<InventoryModel> GetScenarioProductFlows(FragmentNodeResource term, int ex_directionId, out double flow_exch)
        {
            IEnumerable<InventoryModel> Outflows;
            switch (term.NodeTypeID)
            {
                case 1:
                    {
                        // process-- lookup exchange and outflows separately
                        var _flow_exch = _processFlowService.FlowExchange((int)term.ProcessID, term.TermFlowID, ex_directionId);
                        if (_flow_exch == null)
                        {
                            throw new ArgumentNullException("Process inflow not found!");
                        }
                        flow_exch = (double)_flow_exch;

                        Outflows = _processFlowService.GetDependencies((int)term.ProcessID, term.TermFlowID, ex_directionId);
                        break;
                    }
                case 2:
                    {
                        IFragmentTraversalV2 recursive_traversal = new FragmentTraversalV2(_fragmentFlowService,
                            _nodeCacheService,
                            _processFlowService,
                            _flowFlowPropertyService,
                            _dependencyParamService,
                            _unitOfWork);
                        // fragment-- all together
                        // first, traverse the fragment -- store results in cache
                        recursive_traversal.Traverse((int)term.SubFragmentID, term.ScenarioID);
                        // access the cache to determine outflow amounts
                        Outflows = _fragmentFlowService.GetDependencies((int)term.SubFragmentID,term.TermFlowID,ex_directionId,
                            out flow_exch, term.ScenarioID);
                        break;
                    }
                default:
                    {
                        Outflows = new List<InventoryModel>();
                        flow_exch = 1;
                        break;
                    }
            }
            return Outflows;
        }            
            
    }
}


/* *********************
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
*************** */
