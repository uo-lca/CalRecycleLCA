using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public interface ICacheManager
    {
        List<int> InitializeCache();
        ScenarioGroupResource CreateScenarioGroup(ScenarioGroupResource postdata);
        int CreateScenario(ScenarioResource post, int refScenarioId = Scenario.MODEL_BASE_CASE_ID);
        void DeleteScenario(int scenarioId);
        bool ImplementScenarioChanges(int scenarioId, CacheTracker cacheTracker);

        void ClearNodeCacheByScenario(int scenarioId);
        //void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);
        void ClearScoreCacheByScenario(int scenarioId);
        //void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);
        void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId, int lciaMethodId);
    }
}
