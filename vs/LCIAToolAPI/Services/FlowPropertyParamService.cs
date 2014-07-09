using Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{

    public class FlowPropertyParamService : Service<FlowPropertyParam>, IFlowPropertyParamService
    {
          private readonly IRepository<FlowPropertyParam> _repository;

          public FlowPropertyParamService(IRepository<FlowPropertyParam> repository)
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
