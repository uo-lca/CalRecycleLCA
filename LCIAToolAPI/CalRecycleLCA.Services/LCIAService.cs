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
        IEnumerable<LCIAModel> ComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = 0);
        IEnumerable<LCIAModel> OldComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = 0);
    }

    public class LCIAService : Service<LCIA>, ILCIAService
    {
        private readonly IRepositoryAsync<LCIA> _repository;

        public LCIAService(IRepositoryAsync<LCIA> repository)
            : base(repository)
        {
            _repository = repository;            
        }

        public IEnumerable<LCIAModel> ComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = 0)
        {
            return _repository.ComputeLCIA(inventory, lciaMethodId, scenarioId);
        }
        public IEnumerable<LCIAModel> OldComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = 0)
        {
            return _repository.OldComputeLCIA(inventory, lciaMethodId, scenarioId);
        }
    }
}
