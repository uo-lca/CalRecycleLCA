using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IScenarioBackgroundService : IService<ScenarioBackground>
    {
    }

    public class ScenarioBackgroundService : Service<ScenarioBackground>, IScenarioBackgroundService
    {
        public ScenarioBackgroundService(IRepositoryAsync<ScenarioBackground> repository)
            : base(repository)
        {

        }
    }

}
