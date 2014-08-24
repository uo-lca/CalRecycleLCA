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
    public class FlowPropertyEmissionService : Service<FlowPropertyEmission>, IFlowPropertyEmissionService
    {
        [Inject]
        private readonly IRepository<FlowPropertyEmission> _repository;


        public FlowPropertyEmissionService(IRepository<FlowPropertyEmission> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
