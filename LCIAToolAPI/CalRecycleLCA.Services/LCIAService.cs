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
    public interface ILCIAService : IService<LCIA>
    {
        IEnumerable<LCIAModel> ComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAModel> OldComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
    }

    public class LCIAService : Service<LCIA>, ILCIAService
    {
        private readonly IRepositoryAsync<LCIA> _repository;

        public LCIAService(IRepositoryAsync<LCIA> repository)
            : base(repository)
        {
            _repository = repository;            
        }

        public IEnumerable<LCIAModel> ComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.ComputeLCIA(inventory, lciaMethodId, scenarioId);
        }
        public IEnumerable<LCIAModel> OldComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.OldComputeLCIA(inventory, lciaMethodId, scenarioId);
        }
    }
}
