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
        void ClearNodeCacheByScenario(int scenarioId = 0);
        void ClearNodeCacheByScenarioAndFragment(int scenarioId = 0, int fragmentId = 0);
    }
}
