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
    public class ProcessFlowService : Service<ProcessFlow>, IProcessFlowService
    {
         [Inject]
        private readonly IRepository<ProcessFlow> _repository;


         public ProcessFlowService(IRepository<ProcessFlow> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
