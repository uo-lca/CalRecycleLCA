using LcaDataModel;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IScoreCacheService : IService<ScoreCache>
    {
        void ClearScoreCacheByScenario(int scenarioId);
        void ClearScoreCacheByScenario(int scenarioId, List<int> fragmentFlows);

        //void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);

        void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId, int lciaMethodID);

        void ClearScoreCacheForFragments(int scenarioId);
        void ClearScoreCacheForParentFragments(List<int> fragmentIds, int scenarioId);

        IEnumerable<ScoreCache> GetScenarioCaches(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<ScoreCache> GetFragmentFlowCaches(int fragmentFlowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
    }
}
