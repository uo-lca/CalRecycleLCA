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
    public interface IScenarioService : IService<Scenario>
    {
    }

    public class ScenarioService : Service<Scenario>, IScenarioService
    {
        public ScenarioService(IRepositoryAsync<Scenario> repository)
            : base(repository)
        {

        }
    }
}

