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

        /*
        private static FlowTerminationModel GetFragmentNodeProcess(FragmentFlow ff, int scenarioId)
        {
            var fragmentNode = ff.FragmentNodeProcesses.First();
            if (scenarioId != Scenario.MODEL_BASE_CASE_ID)
            {
                int? subsId = fragmentNode.ProcessSubstitutions.Where(x => x.ScenarioID == scenarioId)
                    .Select(a => a.ProcessID).FirstOrDefault();

                if (subsId != null && subsId != 0)
                    fragmentNode.ProcessID = (int)subsId;
            }

            return new FlowTerminationModel
            {
                //ILCDEntityID = fragmentNode.FragmentNodeProcessID,
                NodeTypeID = 1,
                ScenarioID = scenarioId,
                ProcessID = fragmentNode.ProcessID,
                TermFlowID = fragmentNode.FlowID
            };
        }
         * */

        private static FlowTerminationModel LGetFragmentNodeProcess(this IRepository<FragmentFlow> repository,
            int ffid, int scenarioId)
        {
            var Dflt = repository.GetRepository<FragmentNodeProcess>().Queryable()
                .Where(f => f.FragmentFlowID == ffid)
                .First();
            int pId = Dflt.ProcessID;
            var Subs = repository.GetRepository<ProcessSubstitution>().Queryable()
                .Where(ps => ps.ScenarioID == scenarioId)
                .Where(ps => ps.FragmentNodeProcess.FragmentFlowID == ffid)
                .FirstOrDefault();
            if (Subs != null)
                pId = Subs.ProcessID;
            return new FlowTerminationModel()
            {
                ILCDEntityID = repository.GetRepository<Process>().Queryable().Where(p => p.ProcessID == pId)
                    .Select(p => p.ILCDEntityID).First(),
                ScenarioID = scenarioId,
                NodeTypeID = 1,
                ProcessID = pId,
                TermFlowID = Dflt.FlowID,
                BalanceFFID = Dflt.ConservationFragmentFlowID
            };
        }
        private static FlowTerminationModel LGetFragmentNodeFragment(this IRepository<FragmentFlow> repository,
            int ffid, int scenarioId)
        {
            var Dflt = repository.GetRepository<FragmentNodeFragment>().Queryable()
                .Where(f => f.FragmentFlowID == ffid)
                .First();
            int fId = Dflt.SubFragmentID;
            var Subs = repository.GetRepository<FragmentSubstitution>().Queryable()
                .Where(ps => ps.ScenarioID == scenarioId)
                .Where(ps => ps.FragmentNodeFragmentID == Dflt.FragmentNodeFragmentID)
                .FirstOrDefault();
            if (Subs != null)
                fId = Subs.SubFragmentID;
            return new FlowTerminationModel()
            {
                ILCDEntityID = repository.GetRepository<Fragment>().Queryable().Where(f => f.FragmentID == fId)
                    .Select(f => f.ILCDEntityID).First(),
                ScenarioID = scenarioId,
                NodeTypeID = 2,
                SubFragmentID = fId,
                TermFlowID = Dflt.FlowID
            };
        }

        /*
        private static FlowTerminationModel GetFragmentNodeSubFragment(FragmentFlow ff, int scenarioId)
        {
            var fragmentNode = ff.FragmentNodeFragments.First();
            if (scenarioId != Scenario.MODEL_BASE_CASE_ID)
            {
                int? subsId = fragmentNode.FragmentSubstitutions.Where(x => x.ScenarioID == scenarioId)
                    .Select(a => a.SubFragmentID).FirstOrDefault();

                if (subsId != null && subsId != 0)
                    fragmentNode.SubFragmentID = (int)subsId;
            }

            return new FlowTerminationModel
            {
                //ILCDEntityID = fragmentNode.FragmentNodeFragmentID,
                NodeTypeID = 2,
                ScenarioID = scenarioId,
                SubFragmentID = fragmentNode.SubFragmentID,
                TermFlowID = fragmentNode.FlowID
            };
        }
         * */

        /*
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
         * */

        public static IEnumerable<FragmentFlow> LGetFlowsByFragment(this IRepositoryAsync<FragmentFlow> repository,
            int fragmentId)
        {
            return repository.Queryable().Where(k => k.FragmentID == fragmentId);
        }

        /*
        public static IEnumerable<FragmentFlow> GetLCIAFlows(this IRepositoryAsync<FragmentFlow> repository,
            int fragmentId)
        {
            return repository.Query(k => k.FragmentID == fragmentId && k.NodeTypeID != 3)
                //.Include(k => k.FragmentNodeProcesses)
                .Include(k => k.FragmentNodeProcesses.Select(p => p.ProcessSubstitutions))
                //.Include(k => k.FragmentNodeFragments)
                .Include(k => k.FragmentNodeFragments.Select(p => p.FragmentSubstitutions))
                .Select().ToList();
        }
         * */

        /*
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
         * */

        public static IEnumerable<FragmentFlowResource> LGetCachedFlows(this IRepositoryAsync<FragmentFlow> repository,
            int fragmentId, int scenarioId)
        {
            //var inFlow = repository.GetInFlow(fragmentId, scenarioId);
            var ff = repository.LGetFlowsByFragment(fragmentId)
                .Join(repository.GetRepository<NodeCache>().Queryable().Where(nc => nc.ScenarioID == scenarioId),
                    f => f.FragmentFlowID,
                    nc => nc.FragmentFlowID,
                    (f, nc) => new { f, nc })
                .Select(fr => new FragmentFlowResource()
                {
                    FragmentID = fragmentId,
                    FragmentFlowID = fr.f.FragmentFlowID,
                    FragmentStageID = fr.f.FragmentStageID,
                    Name = fr.f.Name,
                    ShortName = fr.f.ShortName,
                    NodeType = Enum.GetName(typeof(NodeTypeEnum), (NodeTypeEnum)fr.f.NodeTypeID),
                    FlowID = fr.f.FlowID, // ?? inFlow.FlowID,
                    Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)fr.f.DirectionID),
                    ParentFragmentFlowID = fr.f.ParentFragmentFlowID,
                    NodeWeight = fr.nc.NodeWeight,
                    FlowPropertyMagnitudes = //(fr.f.FlowID == null)
                    //? repository.GetRepository<FlowFlowProperty>()
                    //    .GetFlowPropertyMagnitudes(inFlow.FlowID, scenarioId, fr.nc.FlowMagnitude).ToList()
                    //: 
                    repository.GetRepository<FlowFlowProperty>()
                        .GetFlowPropertyMagnitudes((int)fr.f.FlowID, scenarioId, fr.nc.FlowMagnitude).ToList()
                });
            return ff;//.Where(f => f.NodeWeight != null);
        }
        
        /// <summary>
        /// Lazy flow termination-- FNF and FNP are looked up now
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="ff"></param>
        /// <param name="scenarioId"></param>
        /// <param name="doBackground"></param>
        /// <returns></returns>
        public static FlowTerminationModel LTerminate(this IRepository<FragmentFlow> repository, 
						     NodeCacheModel ff, int scenarioId, bool doBackground)
        {
            var fragmentNode = new FlowTerminationModel();
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
                    //fragmentNode.ILCDEntityID = ff.FragmentFlowID;
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
                        //fragmentNode.ILCDEntityID = ff.FragmentFlowID;
                        fragmentNode.ScenarioID = scenarioId;
                        fragmentNode.TermFlowID = inFlowId;
                    }
		            break;
        	    }
  	        }
	        return fragmentNode;
	    }

        /*
        ///** ************************
        /// <summary>
        /// Eager flow termination - FNF and FNP are pre-fetched with the FragmentFlow objects.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="ff"></param>
        /// <param name="scenarioId"></param>
        /// <param name="doBackground"></param>
        /// <returns></returns>
        public static FlowTerminationModel Terminate(this IRepository<FragmentFlow> repository, 
						     FragmentFlow ff, int scenarioId, bool doBackground)
	    {
	        var fragmentNode = new FlowTerminationModel();
            int inFlowId;
            fragmentNode.NodeTypeID = ff.NodeTypeID;
	    
	        switch (ff.NodeTypeID)
	        {   
                case 1: 
                {
                    fragmentNode = //repository.LGetFragmentNodeProcess(ff.FragmentFlowID, scenarioId);
                        GetFragmentNodeProcess(ff, scenarioId);
		            break;
                }
    	        case 2:
	            {
		            fragmentNode = //repository.LGetFragmentNodeFragment(ff.FragmentFlowID, scenarioId);
                        GetFragmentNodeSubFragment(ff, scenarioId);
		            break;
	            }
    	        case 3:
	            {
                    //if (ff.FlowID == null)
                    //{
                    //    throw new ArgumentNullException("FragmentFlow.FlowID must be set!");
                    //}
                    //fragmentNode.ILCDEntityID = ff.FragmentFlowID;
            		fragmentNode.ScenarioID = scenarioId;
                    fragmentNode.TermFlowID = ff.FlowID; 
		            break;
        	    }
    	        default:
	            {
                    //if (ff.FlowID == null)
                    //{
                    //    throw new ArgumentNullException("FragmentFlow.FlowID must be set!");
                    //}
                    //else 
                    inFlowId = ff.FlowID;
                    if (doBackground)
                    {
                        fragmentNode = repository.GetRepository<Background>()
                            .ResolveBackground(inFlowId, ff.DirectionID, scenarioId);
                    }
                    else
                    {
                        //fragmentNode.ILCDEntityID = ff.FragmentFlowID;
                        fragmentNode.ScenarioID = scenarioId;
                        fragmentNode.TermFlowID = inFlowId;
                    }
		            break;
        	    }
  	        }
	        return fragmentNode;
	    }
         **************** */

        private static int comp(int direction)
        {
            int compdir = 1;
            if (direction == 1)
                compdir = 2;
            return compdir;
        }

        public static InventoryModel GetInFlow(this IRepository<FragmentFlow> repository,
                     int fragmentId, int scenarioId)
        {
            var inFlow = repository.Queryable()
                .Where(ff => ff.FragmentID == fragmentId)
                .Where(ff => ff.ParentFragmentFlowID == null).Select(ff =>
                new NodeCacheModel
                {
                    FragmentID = fragmentId,
                    FragmentFlowID = ff.FragmentFlowID,
                    NodeTypeID = ff.NodeTypeID,
                    FlowID = ff.FlowID,
                    DirectionID = ff.DirectionID
                }).First();

            var termFlow = repository.LTerminate(inFlow, scenarioId, false).TermFlowID;

            return new InventoryModel
            {
                FlowID = termFlow,
                DirectionID = comp(inFlow.DirectionID),
                Result = 1.0
            };


        }

        public static double GetNodeScaling(this IRepositoryAsync<FragmentFlow> repository, 
            FlowTerminationModel term, int inFlowId, int directionId, int scenarioId )
        {
            var flow_conv = (double)repository.GetRepository<FlowFlowProperty>()
                .FlowConv(term.TermFlowID, inFlowId, scenarioId);
            if (term.NodeTypeID == 1)
                return flow_conv /
                    (double)repository.GetRepository<ProcessFlow>()
                    .FlowExchange((int)term.ProcessID, term.TermFlowID, directionId);
            else
                return flow_conv;
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
                //.Where(ff => ff.FlowID != null)           // reference flow (null FlowID) is .Unioned below
                .Where(ff => ff.NodeTypeID == 3)          // of type InputOutput
                .GroupJoin(repository.GetRepository<NodeCache>().Queryable().Where(nc => nc.ScenarioID == scenarioId),
                    ff => ff.FragmentFlowID,
                    nc => nc.FragmentFlowID,
                    (ff, nc) => new { ff, nc })         // join to NodeCache to get flow magnitude
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

            return Outflows;
        }

        /// <summary>
        /// Determines the default dependency value for a fragmentflow. not fast.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="fragmentFlowId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public static double? GetDefaultValue(this IRepository<FragmentFlow> repository,
            int fragmentFlowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            FragmentFlow fragmentFlow = repository.Queryable()
                .Where(k => k.FragmentFlowID == fragmentFlowId)
                .Where(k => k.ParentFragmentFlow.NodeTypeID == 1)
                .FirstOrDefault();
            if (fragmentFlow == null)
                return null;

            var ncm = new NodeCacheModel()
            {
                NodeTypeID = 1,
                FragmentFlowID = (int)fragmentFlow.ParentFragmentFlowID
            };

            var parentNode = repository.LTerminate(ncm, scenarioId, false);

            return repository.GetRepository<ProcessFlow>().FlowExchange((int)parentNode.ProcessID, fragmentFlow.FlowID,
                comp(fragmentFlow.DirectionID)); // double comp!
        }
    }
}
