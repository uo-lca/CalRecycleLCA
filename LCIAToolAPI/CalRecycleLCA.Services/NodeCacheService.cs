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

        public void ClearNodeCacheByScenarioAndFragments(List<int> fragmentIds, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            _repository.ClearNodeCacheByScenarioAndFragments(fragmentIds, scenarioId);
        }

        public bool IsCached(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.IsCached(fragmentId, scenarioId);
        }

        public IEnumerable<FlowNodeModel> GetLCIAFlows(int fragmentId, int scenarioId)
        {
            return _repository.GetLCIAFlows(fragmentId, scenarioId).Where(k => k.NodeTypeID != 5);
        }

        public IEnumerable<int> ListParents(List<int> fragids, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // list flows that resolve to the given list of fragmentIds
            return _repository.Queryable().Where(nc => nc.ScenarioID == scenarioId)
                .Where(nc => fragids.Contains(nc.ILCDEntity.Fragments.Select(k => k.FragmentID).FirstOrDefault()))
                .Select(nc => nc.FragmentFlowID).Distinct();
        }

        public List<int> SubFragmentsEncountered(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // list all subfragments of the given fragment found in the cache
            return _repository.Queryable().Where(nc => nc.FragmentFlow.FragmentID == fragmentId)
                .Where(nc => nc.ScenarioID == scenarioId)
                .Where(nc => nc.ILCDEntity.DataType.Name == "Fragment")
                .Select(nc => nc.ILCDEntity.Fragments.Select(a => a.FragmentID).FirstOrDefault()).Distinct().ToList();

        }
    }
}
