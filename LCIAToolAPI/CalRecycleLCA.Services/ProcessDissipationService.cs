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
    public interface IProcessDissipationService : IService<ProcessDissipation>
    {
        bool HasDissipation(int processId);
        IEnumerable<InventoryModel> GetDissipation(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
    }

    public class ProcessDissipationService : Service<ProcessDissipation>, IProcessDissipationService
    {
        private IRepositoryAsync<ProcessDissipation> _repository;

        public ProcessDissipationService(IRepositoryAsync<ProcessDissipation> repository)
            : base(repository)
        {
            _repository = repository;            
        }

        public bool HasDissipation(int processId)
        {
            return _repository.Queryable().Any(k => k.ProcessID == processId);
        }

        /// <summary>
        /// First we pull the inventory flows as IEnumerable-- PD -> FPE.FlowID
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public IEnumerable<InventoryModel> GetDissipation(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            if (HasDissipation(processId))
                return _repository.GetDissipation(processId, scenarioId);
            else
                return new List<InventoryModel>();
        }
    }
}
