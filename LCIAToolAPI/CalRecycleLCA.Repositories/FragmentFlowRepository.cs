using LcaDataModel;
using Repository.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Entities.Models;


namespace CalRecycleLCA.Repositories
{
    public static class FragmentFlowRepository
    {

        private static FragmentNodeResource GetFragmentNodeProcess(FragmentFlow ff, int scenarioId)
        {
            var fragmentNode = ff.FragmentNodeProcesses.First();
            /* repository.GetRepository<FragmentNodeProcess>()
                 .Query(x => x.FragmentFlowID == fragmentFlowId)
                 .Include(x => x.ProcessSubstitutions)
                 .Select().First();
            */
            if (scenarioId != 0)
            {
                int? subsId = fragmentNode.ProcessSubstitutions.Where(x => x.ScenarioID == scenarioId)
                    .Select(a => a.ProcessID).FirstOrDefault();

                if (subsId != null && subsId != 0)
                    fragmentNode.ProcessID = subsId;
            }

            return new FragmentNodeResource
            {
                RefID = fragmentNode.FragmentNodeProcessID,
                NodeTypeID = 1,
                ScenarioID = scenarioId,
                ProcessID = fragmentNode.ProcessID,
                TermFlowID = fragmentNode.FlowID
            };
        }

        private static FragmentNodeResource GetFragmentNodeSubFragment(FragmentFlow ff, int scenarioId)
        {
            var fragmentNode = ff.FragmentNodeFragments.First();
            /*                
                
                            repository.GetRepository<FragmentNodeFragment>()
                            .Query(x => x.FragmentFlowID == fragmentFlowId)
                            .Include(x => x.FragmentSubstitutions)
                            .Select().First();
            */
            if (scenarioId != 0)
            {
                int? subsId = fragmentNode.FragmentSubstitutions.Where(x => x.ScenarioID == scenarioId)
                    .Select(a => a.SubFragmentID).FirstOrDefault();

                if (subsId != null && subsId != 0)
                    fragmentNode.SubFragmentID = subsId;
            }

            return new FragmentNodeResource
            {
                RefID = fragmentNode.FragmentNodeFragmentID,
                NodeTypeID = 2,
                ScenarioID = scenarioId,
                SubFragmentID = fragmentNode.SubFragmentID,
                TermFlowID = fragmentNode.FlowID
            };
        }

        public static IEnumerable<FragmentFlow> GetFragmentFlows(this IRepositoryAsync<FragmentFlow> repository, 
            IEnumerable<int> ffids)
        {
            return repository.Query(k => ffids.Contains(k.FragmentFlowID))
                //.Include(k => k.FragmentNodeProcesses)
                .Include(k => k.FragmentNodeProcesses.Select(p => p.ProcessSubstitutions))
                //.Include(k => k.FragmentNodeFragments)
                .Include(k => k.FragmentNodeFragments.Select(p => p.FragmentSubstitutions))
                .Select().ToList();
        }

        public static IEnumerable<FragmentFlow> GetFlowsByFragment(this IRepositoryAsync<FragmentFlow> repository,
            int fragmentId)
        {
            return repository.Query(k => k.FragmentID == fragmentId)
                //.Include(k => k.FragmentNodeProcesses)
                .Include(k => k.FragmentNodeProcesses.Select(p => p.ProcessSubstitutions))
                //.Include(k => k.FragmentNodeFragments)
                .Include(k => k.FragmentNodeFragments.Select(p => p.FragmentSubstitutions))
                .Select().ToList();
        }

        public static IEnumerable<FragmentFlow> GetCachedFlows(this IRepositoryAsync<FragmentFlow> repository,
            int fragmentId, int scenarioId)
        {
            return repository.Query(k => k.FragmentID == fragmentId)
                                .Include(k => k.FragmentNodeProcesses.Select(p => p.ProcessSubstitutions))
                                .Include(k => k.FragmentNodeFragments.Select(p => p.FragmentSubstitutions))
                                .Include(k => k.NodeCaches)
                                .Select()
                                .Where(kk => kk.NodeCaches.Any(a => a.ScenarioID == scenarioId)).ToList();
        }
        
        ///** ************************
        public static FragmentNodeResource Terminate(this IRepositoryAsync<FragmentFlow> repository, 
						     FragmentFlow ff, int scenarioId, bool doBackground)
	    {
	        var fragmentNode = new FragmentNodeResource();
            int inFlowId;
            fragmentNode.NodeTypeID = ff.NodeTypeID;
	    
	        switch (ff.NodeTypeID)
	        {   
                case 1: 
                {
                    //fragmentNode = repository.GetRepository<FragmentNodeProcess>()
		            //  .GetFragmentNodeProcess(ff.FragmentFlowID, scenarioId);
                    fragmentNode = GetFragmentNodeProcess(ff, scenarioId);
		            break;
                }
    	        case 2:
	            {
		            fragmentNode = //repository.GetRepository<FragmentNodeFragment>()
		                GetFragmentNodeSubFragment(ff, scenarioId);
		            break;
	            }
    	        case 3:
	            {
                    if (ff.FlowID == null)
                    {
                        throw new ArgumentNullException("FragmentFlow.FlowID must be set!");
                    }
                    fragmentNode.RefID = ff.FragmentFlowID;
            		fragmentNode.ScenarioID = scenarioId;
                    fragmentNode.TermFlowID = (int)ff.FlowID; 
		            break;
        	    }
    	        default:
	            {
                    if (ff.FlowID == null)
                    {
                        throw new ArgumentNullException("FragmentFlow.FlowID must be set!");
                    }
                    else inFlowId = (int)ff.FlowID;
                    if (doBackground)
                    {
                        fragmentNode = repository.GetRepository<Background>()
                            .ResolveBackground(inFlowId, ff.DirectionID, scenarioId);
                    }
                    else
                    {
                        fragmentNode.RefID = ff.FragmentFlowID;
                        fragmentNode.ScenarioID = scenarioId;
                        fragmentNode.TermFlowID = inFlowId;
                    }
		            break;
        	    }
  	        }
	        return fragmentNode;
	    }
         //* **************** */

        private static int comp(int direction)
        {
            int compdir = 1;
            if (direction == 1)
                compdir = 2;
            return compdir;
        }

        public static InventoryModel GetInFlow(this IRepositoryAsync<FragmentFlow> repository,
                     int fragmentId, int scenarioId)
        {
            var inFlow = repository.Queryable()
                .Where(ff => ff.FragmentID == fragmentId)
                .Where(ff => ff.FlowID == null).First();

            var termFlow = repository.Terminate(inFlow, scenarioId, false).TermFlowID;

            return new InventoryModel
            {
                FlowID = termFlow,
                DirectionID = comp(inFlow.DirectionID),
                Result = 1.0
            };


        }

        /// <summary>
        /// Given a fragment and a reference inflow, reports fragment InputOutput flows as dependencies. 
        /// There is some complexity involved because of the desire to treat fragments as processes 
        /// (i.e. to allow Fragments to be entered from any InputOutput and not just from the reference flow)
        /// 
        /// The out param inFlowMagnitude reports the exchange value for the named inflow, equivalent to 
        /// ProcessFlowService.FlowExchange()
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="fragmentId"></param>
        /// <param name="flowId"></param>
        /// <param name="ex_directionId"></param>
        /// <param name="inFlowMagnitude"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public static IEnumerable<InventoryModel> GetDependencies(this IRepositoryAsync<FragmentFlow> repository, 
            int fragmentId, int flowId, int ex_directionId, out double inFlowMagnitude, int scenarioId)
        {
            int myDirectionId = comp(ex_directionId);

            // in order to proceed, we need to know the fragment's reference flow
            var fragRefFlow = GetInFlow(repository, fragmentId, scenarioId);

            // first thing to do is determine the flows and magnitudes from FragmentFlow * NodeCache
            var Outflows = repository.Queryable()
                .Where(ff => ff.FragmentID == fragmentId) // fragment flows belonging to this fragment
                .Where(ff => ff.NodeTypeID == 3)          // of type InputOutput
                .Join(repository.GetRepository<NodeCache>().Queryable().Where(nc => nc.ScenarioID == scenarioId),
                    ff => ff.FragmentFlowID,
                    nc => nc.FragmentFlowID,
                    (ff, nc) => new { ff, nc })         // join to NodeCache to get flow magnitude
                    .Select(a => new InventoryModel
                    {
                        FlowID = a.ff.FlowID,
                        DirectionID = a.ff.DirectionID,
                        Result = a.nc.FlowMagnitude
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

            // wow! what a mouthful
            // next thing to do is pull out the out inFlowMagnitude

            var inFlow = Outflows.Where(o => o.FlowID == flowId)
                .Where(o => o.DirectionID == myDirectionId).First();

            inFlowMagnitude = (double)inFlow.Result; // out param

            var cropOutflows = Outflows.Where(p => p != inFlow);

            if (cropOutflows.Count() == Outflows.Count())
                throw new ArgumentException("No inFlow found to exclude!");

            return cropOutflows;



        }
    }
}
