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

        /*
        public FragmentFlow GetFragmentFlow(int fragmentFlowId, int scenarioId = 0)
        {

        }
         */

        public FragmentFlow GetFragmentFlow(int fragmentFlowId)
        {
            return _repository.GetFragmentFlows(new List<int>{fragmentFlowId}).First();
        }
        public IEnumerable<FragmentFlow> GetFragmentFlows(IEnumerable<int> ffids)
        {
            return _repository.GetFragmentFlows(ffids);
        }
        public IEnumerable<FragmentFlow> GetFlowsByFragment(int fragmentId)
        {
            return _repository.GetFlowsByFragment(fragmentId);
        }

        public FragmentNodeResource Terminate(FragmentFlow ff, int scenarioId, bool doBackground = false)
	    {
	        return _repository.Terminate(ff, scenarioId, doBackground);
    	}

        public IEnumerable<InventoryModel> GetDependencies(int fragmentId, int flowId, int ex_directionId, 
            out double inFlowMagnitude, int scenarioId = 0)
        {
            return _repository.GetDependencies(fragmentId, flowId, ex_directionId, out inFlowMagnitude, scenarioId);
        }
    }
}
