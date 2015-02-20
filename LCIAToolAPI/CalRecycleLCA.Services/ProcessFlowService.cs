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
        IEnumerable<InventoryModel> GetProductFlows(int processId);
        IEnumerable<InventoryModel> GetDependencies(int processId, int flowId, int ex_directionId);
        IEnumerable<InventoryModel> GetEmissions(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAFactorResource> GetEmissionSensitivity(int processId, int flowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
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

        public IEnumerable<InventoryModel> GetProductFlows(int processId)
        {
            return _repository.GetProductFlows(processId)
                .Select(k => new InventoryModel()
                {
                    FlowID = k.FlowID,
                    DirectionID = k.DirectionID,
                    Result = k.Result,
                    StDev = k.STDev
                });
        }

        /// <summary>
        /// Returns a list of "dependent" flows from a node with respect to a given reference flow. 
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="flowId">the reference FlowID</param>
        /// <param name="ex_directionId">direction of the reference flow with respect to *parent*</param>
        /// <returns></returns>
        public IEnumerable<InventoryModel> GetDependencies(int processId, int flowId, int ex_directionId)
        {
            var Outflows = _repository.GetProductFlows(processId);

            int myDirectionId = 1;
            if (ex_directionId == 1)
                myDirectionId = 2;

            int refPfId = Outflows
                .Where(pf => pf.FlowID == flowId)
                .Where(pf => pf.DirectionID == myDirectionId)
                .First().ProcessFlowID;

            return Outflows.Where(o => o.ProcessFlowID != refPfId)
                .Select(a => new InventoryModel
                {
                    FlowID = a.FlowID,
                    DirectionID = a.DirectionID,
                    Result = a.Result
                }).ToList();

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

        public IEnumerable<LCIAFactorResource> GetEmissionSensitivity(int processId, int flowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.GetEmissionSensitivity(processId, flowId, scenarioId);
        }


    }
}
