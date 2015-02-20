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
        List<LCIAModel> ComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        List<LCIAModel> ComputeLCIADiss(IEnumerable<InventoryModel> dissipation, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        //IEnumerable<LCIAModel> OldComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAFactorResource> QueryFactors(int LCIAMethodID);
    }

    public class LCIAService : Service<LCIA>, ILCIAService
    {
        private readonly IRepositoryAsync<LCIA> _repository;

        public LCIAService(IRepositoryAsync<LCIA> repository)
            : base(repository)
        {
            _repository = repository;            
        }

        public List<LCIAModel> ComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            List<LCIAModel> model = _repository.ComputeLCIA(inventory, lciaMethodId, scenarioId)
                .Where(k => String.IsNullOrEmpty(k.Geography)).ToList();

            foreach (var k in model)
                k.Result = k.Quantity * k.Factor;

            return model;
        }
        public List<LCIAModel> ComputeLCIADiss(IEnumerable<InventoryModel> dissipation, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            List<LCIAModel> model = _repository.ComputeLCIADiss(dissipation, lciaMethodId, scenarioId)
                .Where(k => String.IsNullOrEmpty(k.Geography)).ToList();

            model.RemoveAll(k => k.Composition == null || k.Dissipation == null);

            foreach (var k in model)
            {
                k.Quantity = (double)k.Composition * (double)k.Dissipation;
                k.Result = k.Quantity * k.Factor;
            }
            return model;
        }
        public IEnumerable<LCIAModel> OldComputeLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.OldComputeLCIA(inventory, lciaMethodId, scenarioId);
        }

        public IEnumerable<LCIAFactorResource> QueryFactors(int lciaMethodId)
        {
            return _repository.QueryFactors(lciaMethodId);
        }
    }
}
