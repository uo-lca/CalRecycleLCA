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
    public class NodeCacheService : Service<NodeCache>, INodeCacheService
    {
        private readonly IRepositoryAsync<NodeCache> _repository;

        public NodeCacheService(IRepositoryAsync<NodeCache> repository)
            : base(repository)
        {
            _repository = repository; 
        }

        public void ClearNodeCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            _repository.ClearNodeCacheByScenario(scenarioId);
        }

        public void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _repository.ClearNodeCacheByScenarioAndFragment(scenarioId, fragmentId);
        }

        public bool IsCached(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.IsCached(fragmentId, scenarioId);
        }
    }
}
