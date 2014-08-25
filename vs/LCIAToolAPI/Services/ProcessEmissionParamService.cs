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
    public class ProcessEmissionParamService : Service<ProcessEmissionParam>, IProcessEmissionParamService
    {
        [Inject]
        private readonly IRepository<ProcessEmissionParam> _repository;


        public ProcessEmissionParamService(IRepository<ProcessEmissionParam> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
