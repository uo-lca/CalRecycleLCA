using LcaDataModel;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface INodeCacheService : IService<NodeCache>
    {
        void ClearNodeCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);
    }
}
