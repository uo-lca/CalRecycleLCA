using Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FlowService : Service<Flow>, IFlowService
    {
        private readonly IRepository<Flow> _repository;

        public FlowService(IRepository<Flow> repository)
            : base(repository)
        {
            _repository = repository;
        }
        
        public IEnumerable<Flow> GetFlowsInFragment(int fragmentId) {
            return null;
        }

    }
}
