﻿using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using CalRecycleLCA.Repositories;

namespace CalRecycleLCA.Services
{
    public interface IFragmentFlowService : IService<FragmentFlow>
    {
        IEnumerable<FragmentFlow> LGetFlowsByFragment(int fragmentId);   // LAZY
        IEnumerable<FragmentFlowResource> GetTerminatedFlows(int fragmentId, int scenarioId);  // LAZY
        NodeCacheModel GetNodeModel(FragmentFlow ff);               // used by LAZY
        //FlowTerminationModel Terminate(FragmentFlowResource ff, int scenarioId, bool doBackground = false);  // used by LAZY
        FlowTerminationModel Terminate(NodeCacheModel ncm, int scenarioId, bool doBackground = false);  // used by LAZY

        //IEnumerable<FlowTerminationModel> GetLCIAFlows(int fragmentId, int scenarioId);          // EAGER
        //FlowTerminationModel Terminate(FragmentFlow ff, int scenarioId, bool doBackground = false); // used by EAGER 

        int GetReferenceFlowID(int fragmentId);
        InventoryModel GetInFlow(int fragmentId);
        IEnumerable<InventoryModel> GetDependencies(int fragmentId, int flowId, int ex_directionId,
            out double inFlowMagnitude, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        double GetNodeScaling(FragmentFlowResource ffr, int scenarioId = Scenario.MODEL_BASE_CASE_ID);

        IEnumerable<FragmentStageResource> GetFragmentStages(int fragmentId);
        IEnumerable<FragmentStageResource> GetRecursiveFragmentStages(int fragmentId);

        IEnumerable<int> ListBalanceFlows(int fragmentId);
        //IEnumerable<int> ListParents(List<int> fragids, int scenarioId = Scenario.MODEL_BASE_CASE_ID);

        //FragmentFlow GetFragmentFlow(int fragmentFlowId);
        //IEnumerable<FragmentFlow> GetFragmentFlows(IEnumerable<int> ffids);
        //IEnumerable<FragmentFlow> GetFlowsByFragment(int fragmentId);
        //IEnumerable<FragmentFlow> GetCachedFlows(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        //IEnumerable<FragmentFlowResource> LGetCachedFlows(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
    }

    public class FragmentFlowService : Service<FragmentFlow>, IFragmentFlowService
    {
        private readonly IRepositoryAsync<FragmentFlow> _repository;

        public FragmentFlowService(IRepositoryAsync<FragmentFlow> repository)
            : base(repository)
        {
            _repository = repository;

        }

        // ***********************************************
        // Methods to query the repository
        // ***********************************************

        /*
        /// <summary>
        /// Get a single FragmentFlow by FFID
        /// </summary>
        /// <param name="fragmentFlowId">FFID</param>
        /// <returns>FragmentFlow</returns>
        public FragmentFlow GetFragmentFlow(int fragmentFlowId)
        {
            return _repository.GetFragmentFlows(new List<int>{fragmentFlowId}).First();
        }
        /// <summary>
        /// Get a list of FragmentFlows, given a list of FFIDs
        /// </summary>
        /// <param name="ffids"></param>
        /// <returns>list of FragmentFlows</returns>
        public IEnumerable<FragmentFlow> GetFragmentFlows(IEnumerable<int> ffids)
        {
            return _repository.GetFragmentFlows(ffids);
        }
        /// <summary>
        /// Get a list of FragmentFlows belonging to a given fragment
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <returns>list of FragmentFlows</returns>
        public IEnumerable<FragmentFlow> GetFlowsByFragment(int fragmentId)
        {
            return _repository.GetFlowsByFragment(fragmentId);
        }
         * 
         * */
        public IEnumerable<FragmentFlow> LGetFlowsByFragment(int fragmentId)
        {
            return _repository.LGetFlowsByFragment(fragmentId);
        }

        /// <summary>
        /// Create a FragmentFlowResource from FragmentFlow.  in the future: pop
        /// </summary>
        /// <param name="ff"></param>
        /// <returns></returns>
        public NodeCacheModel GetNodeModel(FragmentFlow ff)
        {
            return new NodeCacheModel
            {
                FragmentID = ff.FragmentID,
                FragmentFlowID = ff.FragmentFlowID,
                NodeTypeID = ff.NodeTypeID,
                FlowID = ff.FlowID,
                DirectionID = ff.DirectionID,
            };
        }
        /// <summary>
        /// For use by the Resource service to return all info about flows
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public IEnumerable<FragmentFlowResource> GetTerminatedFlows(int fragmentId, int scenarioId)
        {
            return _repository.LGetCachedFlows(fragmentId, scenarioId)
                .Select(f => TerminateInPlace(f, scenarioId, true)).ToList();
        }

        //public IEnumerable<FragmentFlow> GetCachedFlows(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        //{
        //    return _repository.GetCachedFlows(fragmentId, scenarioId);
        //}
        /*
        public IEnumerable<FragmentFlowResource> LGetCachedFlows(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.LGetCachedFlows(fragmentId, scenarioId).ToList();
        }
         * 
         * */
        // ***********************************************
        // Methods to work with FragmentFlows
        // ***********************************************

        /// <summary>
        /// Looks up scenario-specific termination of a FragmentFlow.  Encapsulates 
        /// the FragmentNodeProcess and FragmentNodeFragment tables, as well as their
        /// scenario substitutions (ProcessSubstitution and FragmentSubstitution) assuming 
        /// the required navigation paths were .Included during the query.
        /// </summary>
        /// <param name="ff"></param>
        /// <param name="scenarioId"></param>
        /// <param name="doBackground"></param>
        /// <returns>FragmentNodeResource</returns>
        /*
        public FlowTerminationModel Terminate(FragmentFlow ff, int scenarioId, bool doBackground = false)
	    {
	        return _repository.Terminate(ff, scenarioId, doBackground);
    	}
         * */
        /// <summary>
        /// Same again, but for non-eager-loading.
        /// </summary>
        /// <param name="ff"></param>
        /// <param name="scenarioId"></param>
        /// <param name="doBackground"></param>
        /// <returns></returns>
        private FlowTerminationModel Terminate(FragmentFlowResource ff, int scenarioId, bool doBackground = false)
        {
            return _repository.LTerminate(new NodeCacheModel() 
            { 
                NodeTypeID = Convert.ToInt32(Enum.Parse(typeof(NodeTypeEnum),ff.NodeType)),
                FragmentFlowID = ff.FragmentFlowID,
                FlowID = ff.FlowID,
                DirectionID = Convert.ToInt32(Enum.Parse(typeof(DirectionEnum),ff.Direction)),
            }, scenarioId, doBackground);
        }

        public FlowTerminationModel Terminate(NodeCacheModel ncm, int scenarioId, bool doBackground = false)
        {
            return _repository.LTerminate(ncm, scenarioId, doBackground);
        }

        public InventoryModel GetInFlow(int fragmentId)
        {
            return _repository.GetInFlow(fragmentId, 1);
        }

        private FragmentFlowResource TerminateInPlace(FragmentFlowResource ff, int scenarioId, bool doBackground = false)
        {
            var fn = Terminate(ff, scenarioId, doBackground);
            ff.ProcessID = fn.ProcessID;
            ff.SubFragmentID = fn.SubFragmentID;
            ff.IsBackground = (ff.NodeType == "Background");
            ff.NodeType = Enum.GetName(typeof(NodeTypeEnum), (NodeTypeEnum)fn.NodeTypeID);
            if (ff.FlowID == null)
                ff.FlowID = fn.TermFlowID;
            return ff;
        }

        // ***********************************************
        // Helper / informational methods
        // ***********************************************

        public int GetReferenceFlowID(int fragmentId)
        {
            return _repository.Queryable()
                .Where(k => k.FragmentID == fragmentId)
                .Where(k => k.ParentFragmentFlowID == null)
                .Select(k => k.FragmentFlowID).First();
        }

        private static int comp(int direction)
        {
            int compdir = 1;
            if (direction == 1)
                compdir = 2;
            return compdir;
        }

        /// <summary>
        /// Returns a list of fragment Inputs and Outputs as "dependencies" of a named
        /// input flow by pairing fragmentflows of type 3 with their FlowMagnitudes.  
        /// The fragment's reference flow must be added manually as it also represents an 
        /// InputOutput.  
        /// 
        /// Calling function must supply an inflow, which represents the reference flow of 
        /// the fragment *instance*-- nominally the reference flow but not always.  All 
        /// other flows are modeled as dependencies regardless of flow direction.
        /// 
        /// The out param inFlowMagnitude reports the exchange value for the named inflow, equivalent to 
        /// ProcessFlowService.FlowExchange()
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="flowId">FlowID of reference flow</param>
        /// <param name="ex_directionId">Direction of reference flow with respect to *parent* node</param>
        /// <param name="inFlowMagnitude"></param>
        /// <param name="scenarioId"></param>
        /// <returns>list of InventoryModels (flow, direction, quantity)</returns>
        public IEnumerable<InventoryModel> GetDependencies(int fragmentId, int flowId, int ex_directionId,
            out double inFlowMagnitude, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            var Outflows = _repository.GetProductFlows(fragmentId, scenarioId);
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

        /*
        public IEnumerable<FlowTerminationModel> GetLCIAFlows(int fragmentId, int scenarioId)
        {
            return _repository.GetLCIAFlows(fragmentId, scenarioId);
        }
         * */

        public double GetNodeScaling(FragmentFlowResource ffr, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            FlowTerminationModel term = Terminate(ffr, scenarioId);
            return _repository.GetNodeScaling(term, (int)ffr.FlowID, 
                Convert.ToInt32(Enum.Parse(typeof(DirectionEnum),ffr.Direction)), 
                scenarioId);
        }

        public IEnumerable<FragmentStageResource> GetFragmentStages(int fragmentId)
        {
            return _repository.Queryable().Where(k => k.FragmentID == fragmentId)
                .Where(k => k.FragmentStageID != null)
                .Select(k => new FragmentStageResource()
                {
                    FragmentStageID = (int)k.FragmentStageID,
                    FragmentID = fragmentId,
                    Name = k.FragmentStage.Name
                }).Distinct().ToList();
        }

        public IEnumerable<FragmentStageResource> GetRecursiveFragmentStages(int fragmentId)
        {
            var flatStages = _repository.Queryable().Where(k => k.FragmentID == fragmentId)
                .Where(k => k.FragmentStageID != null)
                .Where(k => k.NodeTypeID != 2)
                .Select(k => new FragmentStageResource()
                {
                    FragmentStageID = (int)k.FragmentStageID,
                    Name = k.FragmentStage.Name
                }).ToList();

            flatStages.AddRange(_repository.Queryable().Where(k => k.FragmentID == fragmentId)
                .Where(k => k.NodeTypeID == 2)
                .Where(k => k.FragmentNodeFragments.FirstOrDefault().Descend == false)
                .Select(k => new FragmentStageResource()
                {
                    FragmentStageID = (int)k.FragmentStageID,
                    Name = k.FragmentStage.Name
                }));

            List<int> subfragments = _repository.Queryable().Where(k => k.FragmentID == fragmentId)
                .Where(k => k.NodeTypeID == 2)
                .Where(k => k.FragmentNodeFragments.FirstOrDefault().Descend == true)
                .Select(k => k.FragmentNodeFragments.FirstOrDefault().SubFragmentID).ToList(); // breaks for Fragment Substitutions

            foreach (int subfrag in subfragments)
                flatStages.AddRange(GetRecursiveFragmentStages(subfrag));

            return flatStages.Distinct().OrderBy(k => k.FragmentStageID).ToList();
        }

        public IEnumerable<int> ListBalanceFlows(int fragmentId)
        {
            // lists all fragmentflows that are balances
            return _repository.GetRepository<FragmentNodeProcess>().Queryable()
                .Where(fnp => fnp.FragmentFlow.FragmentID == fragmentId && fnp.ConservationFragmentFlowID != null)
                .Select(fnp => (int)fnp.ConservationFragmentFlowID);
        }

        /*
        public IEnumerable<int> ListParents(List<int> fragids, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // lists FFIDs that could identify the named fragid as a SubFragment
            var defaultParents = _repository.GetRepository<FragmentNodeFragment>().Queryable()
                .Where(k => fragids.Contains(k.SubFragmentID))
                .Select(k => k.FragmentFlowID).ToList();

            if (scenarioId != Scenario.MODEL_BASE_CASE_ID)
                defaultParents.AddRange(_repository.GetRepository<FragmentSubstitution>().Queryable()
                    .Where(k => k.ScenarioID == scenarioId)
                    .Where(k => fragids.Contains(k.SubFragmentID))
                    .Select(k => k.FragmentNodeFragment.FragmentFlowID).ToList());

            return defaultParents.Distinct();
        }

        */
    }
}
