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
        void ClearScoreCacheByScenario(int scenarioId = 0);

        void ClearScoreCacheByScenarioAndFragment(int scenarioId = 0, int fragmentId = 0);

        void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId = 0, int lciaMethodID = 0);
    }
}
