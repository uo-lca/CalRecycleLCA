using LcaDataModel;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using CalRecycleLCA.Repository;


namespace CalRecycleLCA.Services
{
    public class DependencyParamService : Service<DependencyParam>, IDependencyParamService
    {
        private readonly IRepository<DependencyParam> _repository;

        public DependencyParamService(IRepository<DependencyParam> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<DependencyParam> GetDependencyParams(int scenarioId = 0)
        {
            return _repository.GetDependencyParams(scenarioId);
        }

    }

    
}
