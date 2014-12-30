﻿using LcaDataModel;
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
    /// <summary>
    /// Note 'L' prefix is supposed to use lazy loading instead of eager loading. 
    /// not sure whether it matters though.  could be that I waste all the advantage
    /// in the way LGetFragmentNode* are constructed.
    /// </summary>
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
            if (scenarioId != Scenario.MODEL_BASE_CASE_ID)
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

        private static FragmentNodeResource LGetFragmentNodeProcess(this IRepositoryAsync<FragmentFlow> repository,
            int ffid, int scenarioId)
        {
            var Dflt = repository.GetRepository<FragmentNodeProcess>().Queryable()
                .Where(f => f.FragmentFlowID == ffid)
                .First();
            IEnumerable<ProcessSubstitution> Subs = repository.GetRepository<ProcessSubstitution>().Queryable()
                .Where(ps => ps.FragmentNodeProcessID == Dflt.FragmentNodeProcessID)
                .Where(ps => ps.ScenarioID == scenarioId).ToList();
            return new FragmentNodeResource()
            {
                RefID = Dflt.FragmentNodeProcessID,
                ScenarioID = scenarioId,
                NodeTypeID = 1,
                ProcessID = Subs.Count() == 0 ? Dflt.ProcessID : Subs.First().ProcessID,
                TermFlowID = Dflt.FlowID
            };
        }
        private static FragmentNodeResource LGetFragmentNodeFragment(this IRepositoryAsync<FragmentFlow> repository,
            int ffid, int scenarioId)
        {
            var Dflt = repository.GetRepository<FragmentNodeFragment>().Queryable()
                .Where(f => f.FragmentFlowID == ffid)
                .First();
            IEnumerable<FragmentSubstitution> Subs = repository.GetRepository<FragmentSubstitution>().Queryable()
                .Where(ps => ps.FragmentNodeFragmentID == Dflt.FragmentNodeFragmentID)
                .Where(ps => ps.ScenarioID == scenarioId).ToList();
            return new FragmentNodeResource()
            {
                RefID = Dflt.FragmentNodeFragmentID,
                ScenarioID = scenarioId,
                NodeTypeID = 2,
                SubFragmentID = Subs.Count() == 0 ? Dflt.SubFragmentID : Subs.First().SubFragmentID,
                TermFlowID = Dflt.FlowID
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
            if (scenarioId != Scenario.MODEL_BASE_CASE_ID)
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
        public static IEnumerable<FragmentFlow> LGetFragmentFlows(this IRepositoryAsync<FragmentFlow> repository,
            IEnumerable<int> ffids)
        {
            return repository.Queryable().Where(k => ffids.Contains(k.FragmentFlowID));
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
        public static IEnumerable<FragmentFlow> LGetFlowsByFragment(this IRepositoryAsync<FragmentFlow> repository,
            int fragmentId)
        {
            return repository.Queryable().Where(k => k.FragmentID == fragmentId);
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
        
        public static IEnumerable<FragmentFlowResource> LGetCachedFlows(this IRepositoryAsync<FragmentFlow> repository,
            int fragmentId, int scenarioId)
        {
            var ff = repository.LGetFlowsByFragment(fragmentId)
                .Join(repository.GetRepository<NodeCache>().Queryable().Where(nc => nc.ScenarioID == scenarioId),
                    f => f.FragmentFlowID,
                    nc => nc.FragmentFlowID,
                    (f, nc) => new {f, nc})
                .Select(fr => new FragmentFlowResource() {
                FragmentFlowID = fr.f.FragmentFlowID,
                FragmentStageID = fr.f.FragmentStageID,
                Name = fr.f.Name,
                ShortName = fr.f.ShortName,
                NodeTypeID = fr.f.NodeTypeID,
                FlowID = fr.f.FlowID,
                DirectionID = fr.f.DirectionID,
                ParentFragmentFlowID = fr.f.ParentFragmentFlowID,
                NodeWeight = fr.nc.NodeWeight
            });
            return ff;//.Where(f => f.NodeWeight != null);
        }

        public static FragmentNodeResource Terminate(this IRepositoryAsync<FragmentFlow> repository, 
						     FragmentFlowResource ff, int scenarioId, bool doBackground)
        {
            var fragmentNode = new FragmentNodeResource();
            int inFlowId;
            switch (ff.NodeTypeID)
            { 
                case 1:
                    {
                        fragmentNode = repository.LGetFragmentNodeProcess(ff.FragmentFlowID, scenarioId);
                        break;
                    }
                case 2:
                    {
                        fragmentNode = repository.LGetFragmentNodeFragment(ff.FragmentFlowID, scenarioId);
                        break;
                    }
    	        case 3:
	            {
                    if (ff.FlowID == null)
                    {
                        throw new ArgumentNullException("FragmentFlow.FlowID must be set!");
                    }
                    fragmentNode.NodeTypeID = 3;
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
                    fragmentNode = repository.LGetFragmentNodeProcess(ff.FragmentFlowID, scenarioId);
		            break;
                }
    	        case 2:
	            {
		            fragmentNode = //repository.GetRepository<FragmentNodeFragment>()
		                repository.LGetFragmentNodeFragment(ff.FragmentFlowID, scenarioId);
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
                .Where(ff => ff.ParentFragmentFlowID == null).First();

            var termFlow = repository.Terminate(inFlow, scenarioId, false).TermFlowID;

            return new InventoryModel
            {
                FlowID = termFlow,
                DirectionID = comp(inFlow.DirectionID),
                Result = 1.0
            };


        }

        /// <summary>
        /// Given a fragment, reports fragment InputOutput flows as product flows. 
        /// There is some complexity involved because the fragment's reference flow does not 
        /// appear as an InputOutput and must be added with Union().  Then all individual node
        /// outputs must be grouped by FlowID and direction.
        /// Note that the NodeCache MUST be populated for this function to work.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        /// <returns>InventoryModel (list)</returns>
        public static IEnumerable<InventoryModel> GetProductFlows(this IRepositoryAsync<FragmentFlow> repository, 
            int fragmentId, int scenarioId)
        {
            // in order to proceed, we need to know the fragment's reference flow
            var fragRefFlow = GetInFlow(repository, fragmentId, scenarioId);

            // first thing to do is determine the flows and magnitudes from FragmentFlow * NodeCache
            var Outflows = repository.Queryable()
                .Where(ff => ff.FragmentID == fragmentId) // fragment flows belonging to this fragment
                .Where(ff => ff.FlowID != null)           // reference flow (null FlowID) is .Unioned below
                .Where(ff => ff.NodeTypeID == 3)          // of type InputOutput
                .Join(repository.GetRepository<NodeCache>().Queryable().Where(nc => nc.ScenarioID == scenarioId),
                    ff => ff.FragmentFlowID,
                    nc => nc.FragmentFlowID,
                    (ff, nc) => new { ff, nc })         // join to NodeCache to get flow magnitude
                    .Select(a => new InventoryModel
                    {
                        FlowID = (int)a.ff.FlowID,
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

            return Outflows;
        }
    }
}
