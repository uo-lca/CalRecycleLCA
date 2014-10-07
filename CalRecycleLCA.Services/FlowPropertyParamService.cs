using LcaDataModel;
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
    public class FlowPropertyParamService : Service<FlowPropertyParam>, IFlowPropertyParamService
    {
          private readonly IRepositoryAsync<FlowPropertyParam> _repository;

          public FlowPropertyParamService(IRepositoryAsync<FlowPropertyParam> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<FlowPropertyParam> GetFlowPropertyParams(int scenarioId = 0)
        {
            return _repository.GetFlowPropertyParams(scenarioId);
        }
    }
}
