using LcaDataModel;
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
        FragmentFlow GetFragmentFlow(int fragmentFlowId);
        IEnumerable<FragmentFlow> GetFragmentFlows(IEnumerable<int> ffids);
        IEnumerable<FragmentFlow> GetFlowsByFragment(int fragmentId);
        FragmentNodeResource Terminate(FragmentFlow ff, int scenarioId, bool doBackground = false);
        IEnumerable<InventoryModel> GetDependencies(int fragmentId, int flowId, int ex_directionId, 
            out double inFlowMagnitude, int scenarioId = 0);
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
        public FragmentNodeResource Terminate(FragmentFlow ff, int scenarioId, bool doBackground = false)
	    {
	        return _repository.Terminate(ff, scenarioId, doBackground);
    	}

        // ***********************************************
        // Helper / informational methods
        // ***********************************************
        
        /// <summary>
        /// Returns a list of fragment Inputs and Outputs as "dependencies" of a named
        /// input flow by pairing fragmentflows of type 3 with their FlowMagnitudes.  
        /// The fragment's reference flow must be added manually as it also represents an 
        /// InputOutput.  
        /// 
        /// Calling function must supply an inflow, which represents the reference flow of 
        /// the fragment *instance*-- nominally the reference flow but not always.  All 
        /// other flows are modeled as dependencies regardless of flow direction.
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="flowId"></param>
        /// <param name="ex_directionId"></param>
        /// <param name="inFlowMagnitude"></param>
        /// <param name="scenarioId"></param>
        /// <returns>list of InventoryModels (flow, direction, quantity)</returns>
        public IEnumerable<InventoryModel> GetDependencies(int fragmentId, int flowId, int ex_directionId, 
            out double inFlowMagnitude, int scenarioId = 0)
        {
            return _repository.GetDependencies(fragmentId, flowId, ex_directionId, out inFlowMagnitude, scenarioId);
        }
    }
}
