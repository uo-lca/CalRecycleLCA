using Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FlowFlowPropertyService : Service<FlowFlowProperty>, IFlowFlowPropertyService
    {
          private readonly IRepository<FlowFlowProperty> _repository;

          public FlowFlowPropertyService(IRepository<FlowFlowProperty> repository)
            : base(repository)
        {
            _repository = repository;
        }

    }
}
