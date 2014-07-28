using LcaDataModel;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{

    public class ScenarioParamService : Service<ScenarioParam>, IScenarioParamService
    {
        [Inject]
        private readonly IRepository<ScenarioParam> _repository;


        public ScenarioParamService(IRepository<ScenarioParam> repository)
            : base(repository)
        {
            _repository = repository;
        }

       

    }
}
