using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalRecycleLCA.Repositories;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public interface IProcessFlowService : IService<ProcessFlow>
    {
        double? FlowExchange(int processId, int flowId, int ex_directionId); // opposite of ProcessFlow.DirectionID
        IEnumerable<InventoryModel> GetDependencies(int processId, int flowId, int ex_directionId);
        IEnumerable<InventoryModel> GetEmissions(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        // IEnumerable<InventoryModel> GetEmissionsOld(int processId, int scenarioId);
    }

    public class ProcessFlowService : Service<ProcessFlow>, IProcessFlowService
    {
        private readonly IRepositoryAsync<ProcessFlow> _repository;

        public ProcessFlowService(IRepositoryAsync<ProcessFlow> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public double? FlowExchange(int processId, int flowId, int ex_directionId)
        {
            return _repository.FlowExchange(processId, flowId, ex_directionId);
        }

        public IEnumerable<InventoryModel> GetDependencies(int processId, int flowId, int ex_directionId)
        {
            return _repository.GetDependencies(processId, flowId, ex_directionId);
        }

        public IEnumerable<InventoryModel> GetEmissions(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.GetEmissions(processId, scenarioId);
        }

        /*
        public IEnumerable<InventoryModel> GetEmissionsOld(int processId, int scenarioId)
        {
            return _repository.GetEmissionsOld(processId, scenarioId);
        }
         * */
    }
}
