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

        private List<NodeCacheModel> nodeCaches;


        public FragmentTraversalV2(
            IFragmentFlowService fragmentFlowService,
            INodeCacheService nodeCacheService,
            IProcessFlowService processFlowService,
            IFlowFlowPropertyService flowFlowPropertyService,
            IDependencyParamService dependencyParamService)
        {
            _fragmentFlowService = fragmentFlowService;
            _nodeCacheService = nodeCacheService;
            _processFlowService = processFlowService;
            _flowFlowPropertyService = flowFlowPropertyService;
            _dependencyParamService = dependencyParamService;

            nodeCaches = new List<NodeCacheModel>();
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
        public IEnumerable<NodeCacheModel> EnterTraversal(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            nodeCaches.Clear();
            return Traverse(fragmentId, scenarioId); 
        }
            

        private IEnumerable<NodeCacheModel> Traverse(int fragmentId, int scenarioId)
        {
            // we only enter this function if traversal is required.

            var fragmentFlows = _fragmentFlowService.LGetFlowsByFragment(fragmentId);
            //var dependencyParams = _dependencyParamService.Query(dp => dp.Param.ScenarioID == scenarioId).Select().ToList();

            float activity = 1;

            int refFlow = fragmentFlows.Where(k => k.ParentFragmentFlowID == null).Select(k => k.FragmentFlowID).First();

            NodeRecurse(fragmentFlows, refFlow, scenarioId, activity);

            return nodeCaches;

        }

        public IEnumerable<FragmentFlowResource> SensitivityTraverse(FragmentFlowResource ffr, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            nodeCaches.Clear(); // reset these

            // here, traversal is required because dependency parameter is zero in default scenario
            var fragmentFlows = _fragmentFlowService.LGetFlowsByFragment(ffr.FragmentID);
            //var dependencyParams = _dependencyParamService.Query(dp => dp.Param.ScenarioID == scenarioId).Select().ToList();

            float activity = 1;

            NodeRecurse(fragmentFlows, ffr.FragmentFlowID, scenarioId, activity);

            return nodeCaches.Where(n => n.FragmentID == ffr.FragmentID) // un-traversed sub-fragments are an unhandled issue at present 
                .Select(n => new FragmentFlowResource()
                {
                    FragmentFlowID = n.FragmentFlowID,
                    FragmentStageID = fragmentFlows.Where(k => k.FragmentFlowID == n.FragmentFlowID)
                        .Select(k => k.FragmentStageID).First(),
                    NodeWeight = n.NodeWeight
                });

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
                               // IEnumerable<DependencyParam> ff_param,
                                int fragmentFlowId, int scenarioId, double flowMagnitude)
        {
            // TODO: enable the following and remove IsCached check in GetScenarioProductFlows
            //if (nodeCaches.Any(n => n.FragmentFlowID == fragmentFlowId))
              //  return; // bail out!
            var theFragmentFlow = _fragmentFlowService.GetNodeModel(ff
                .Where(k => k.FragmentFlowID == fragmentFlowId).First());
            
            double nodeWeight;
            int? ilcdEntityId;
            var outFlows = GetScenarioProductFlows(theFragmentFlow, scenarioId, out nodeWeight, out ilcdEntityId);
            nodeWeight *= flowMagnitude;

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
                if (outLinks.Count() > outFlows.Count()) // TODO -- this should be !=
                {
                    throw new ArgumentException("OutFlows and OutLinks don't reconcile! FFID: " + Convert.ToString(fragmentFlowId));
                }

                // abandon recursion if no recursive steps-- but foreach should just not run if outLinks is empty
                foreach (var item in outLinks)
                {
                    //var this_param = ff_param.Where(dp => dp.FragmentFlowID == item.FragmentFlowID).FirstOrDefault();
                    double resultVal = (double)outFlows
                        .Where(o => o.FlowID == item.FlowID && o.DirectionID == item.DirectionID)
                        .First().Result; // First() should generate exception if no match is found

                    //if (this_param != null)
                    //    resultVal = this_param.Value;

                    NodeRecurse(ff, item.FragmentFlowID, scenarioId, nodeWeight * resultVal);
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

            
            nodeCaches.Add(new NodeCacheModel()
            {
                FragmentID = (int)theFragmentFlow.FragmentID,
                FragmentFlowID = fragmentFlowId,
                NodeTypeID = theFragmentFlow.NodeTypeID,
                FlowID = theFragmentFlow.FlowID,
                DirectionID = theFragmentFlow.DirectionID,
                ScenarioID = scenarioId,
                ILCDEntityID = ilcdEntityId,
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
        public IEnumerable<InventoryModel> GetScenarioProductFlows(NodeCacheModel theFragmentFlow, int scenarioId, out double nodeWeight, out int? ilcdEntityId)
        {

            FlowTerminationModel term = _fragmentFlowService.Terminate(theFragmentFlow,scenarioId, true); // resolve background

            // first, calculate node weight
            double? flow_conv = _flowFlowPropertyService.FlowConv(theFragmentFlow.FlowID, term.TermFlowID, scenarioId);
            if (flow_conv == null)
            {
                throw new ArgumentNullException("Flow conversion was not found and cannot be null");
            }


            double flow_exch;
            // and incoming exchange
            // note this is not subject to parameter adjustment            
            IEnumerable<InventoryModel> Outflows;

            switch (term.NodeTypeID)
            {
                case 1:
                    {
                        // process-- lookup exchange and outflows separately
                        //var _flow_exch = _processFlowService.FlowExchange((int)term.ProcessID, term.TermFlowID, theFragmentFlow.DirectionID);
                        //if (_flow_exch == null)
                        //{
                        //    throw new ArgumentNullException("Process inflow not found!");
                        //}
                        //flow_exch = (double)_flow_exch;

                        //Outflows = _processFlowService.GetDependencies((int)term.ProcessID, term.TermFlowID, theFragmentFlow.DirectionID);

                        if (term.BalanceFFID == null)
                        {
                            // no conservation performed
                            var FFM = _processFlowService.LookupDependencies(theFragmentFlow.FragmentFlowID, term, scenarioId).ToList();
                            flow_exch = (double)FFM.Where(k => k.FlowID == term.TermFlowID && k.DirectionID != theFragmentFlow.DirectionID)
                                .Select(k => k.Result).First();
                            Outflows = FFM.Where(k => k.FragmentFlowID != null).ToList();
                        }
                        else
                        {
                            var CM = _processFlowService.LookupConservationFlows(theFragmentFlow.FragmentFlowID, term, scenarioId).ToList();
                            flow_exch = (double)CM.Where(k => k.FlowID == term.TermFlowID && k.DirectionID != theFragmentFlow.DirectionID)
                                .Select(k => k.Result).First();

                            var balanceDir = CM.Where(k => k.FragmentFlowID == term.BalanceFFID).Select(k => k.DirectionID).First();

                            var balance = CM.Where(k => k.DirectionID != balanceDir).Sum(k => k.FlowPropertyResult)
                                        - CM.Where(k => k.DirectionID == balanceDir).Where(k => k.FragmentFlowID != term.BalanceFFID)
                                        .Sum(k => k.FlowPropertyResult);

                            foreach (var bf in CM.Where(k => k.FragmentFlowID == term.BalanceFFID))
                                bf.FlowPropertyResult = balance;

                            Outflows = CM.Where(k => k.FragmentFlowID != null).ToList();

                        }

                        break;
                    }
                case 2:
                    {
                        // does the subfragment exist in the cache? if so, use that
                        if (_nodeCacheService.IsCached((int)term.SubFragmentID,term.ScenarioID))
                            Outflows = _fragmentFlowService.GetDependencies((int)term.SubFragmentID, term.TermFlowID, theFragmentFlow.DirectionID,
                                out flow_exch, term.ScenarioID);
                        else
                        {
                            // have we already traversed this subfragment in this unit of work?
                            int subFragRefFlow = _fragmentFlowService
                                .Query(k => k.FragmentID == term.SubFragmentID && k.ParentFragmentFlowID == null)
                                .Select(k => k.FragmentFlowID).First();
                            if (!nodeCaches.Any(k => k.FragmentFlowID == subFragRefFlow))
                            {
                                // if not, we need to
                                Traverse((int)term.SubFragmentID, term.ScenarioID);
                            }
                            // access the cache to determine outflow amounts
                            //Outflows = _fragmentFlowService.GetDependencies((int)term.SubFragmentID,term.TermFlowID,ex_directionId,
                            //    out flow_exch, term.ScenarioID);
                            Outflows = GetDependencies((int)term.SubFragmentID, term.TermFlowID, theFragmentFlow.DirectionID,
                                out flow_exch, term.ScenarioID);
                        }
                        break;
                    }
                default:
                    {
                        Outflows = new List<InventoryModel>();
                        flow_exch = 1;
                        break;
                    }
            }
            if (flow_exch == 0)
            {
                throw new ArgumentException("The inflow result cannot be 0");
            }

            nodeWeight = (double)flow_conv / flow_exch;
            ilcdEntityId = term.ILCDEntityID;

            return Outflows;
        }


        private static int comp(int direction)
        {
            int compdir = 1;
            if (direction == 1)
                compdir = 2;
            return compdir;
        }


        private IEnumerable<InventoryModel> GetDependencies(int fragmentId, int flowId, int ex_directionId,
            out double inFlowMagnitude, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // private re-implementation of _FragmentFlowService.GetDependencies that uses local nodeCaches
            var fragRefFlow = _fragmentFlowService.GetInFlow(fragmentId);

            var Outflows = _fragmentFlowService.Queryable()
                .Where(ff => ff.FragmentID == fragmentId)
                //.Where(ff => ff.FlowID != null)           // reference flow (null FlowID) is .Unioned below
                .Where(ff => ff.NodeTypeID == 3).ToList()          // of type InputOutput
                .GroupJoin(nodeCaches,
                    ff => ff.FragmentFlowID,
                    nc => nc.FragmentFlowID,
                    (ff,nc) => new { ff, nc })
                .SelectMany(d => d.nc.DefaultIfEmpty(), (d,nc) => new InventoryModel
                    {
                        FlowID = d.ff.FlowID,
                        DirectionID = d.ff.DirectionID,
                        Result = nc == null ? 0.0 : nc.FlowMagnitude
                    }).ToList()                         // into List<InventoryModel>
                .Union(new List<InventoryModel> { fragRefFlow }) // add fragment ReferenceFlow
                .GroupBy(a => new                   // group by Flow and Direction
                    {
                        a.FlowID,
                        a.DirectionID
                    })
                .Select(group => new InventoryModel
                    {
                        FlowID = group.Key.FlowID,
                        DirectionID = group.Key.DirectionID,
                        Result = group.Sum(a => a.Result)
                    }).ToList();

            // need to look for unvisited 

            var myDirectionId = comp(ex_directionId);

            // next thing to do is pull out the out inFlowMagnitude
            var inFlow = Outflows.Where(o => o.FlowID == flowId)
                .Where(o => o.DirectionID == myDirectionId).First();

            inFlowMagnitude = (double)inFlow.Result; // out param

            // short-circuit OR is correct: only exclude flows where both filters match
            var cropOutflows = Outflows.Where(p => p.FlowID != inFlow.FlowID || p.DirectionID !=inFlow.DirectionID);

            if ( (1 + cropOutflows.Count()) != Outflows.Count())
                throw new ArgumentException("No inFlow found to exclude!");

            return cropOutflows;

        }
    }
}

