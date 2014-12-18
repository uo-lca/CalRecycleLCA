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
        void ClearScoreCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID);

        void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);

        void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int lciaMethodID = 0);
    }
}
