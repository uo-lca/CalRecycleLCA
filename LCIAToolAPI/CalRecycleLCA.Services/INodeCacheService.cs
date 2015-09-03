using LcaDataModel;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public interface INodeCacheService : IService<NodeCache>
    {
        void ClearNodeCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        void ClearNodeCacheByScenarioAndFragments(List<int> fragmentIds, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        bool IsCached(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FlowNodeModel> GetLCIAFlows(int fragmentId, int scenarioId);
    }
}
