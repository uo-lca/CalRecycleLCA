using LcaDataModel;
using Repository;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalRecycleLCA.Repository;

namespace CalRecycleLCA.Services
{
    public class DependencyParamService : Service<DependencyParam>, IDependencyParamService
    {
        private readonly IRepositoryAsync<DependencyParam> _repository;

        public DependencyParamService(IRepositoryAsync<DependencyParam> repository) : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<DependencyParam> GetDependencyParams(int scenarioId = 0)
        {
            return _repository.GetDependencyParams(scenarioId);
        }

    }
    
}
