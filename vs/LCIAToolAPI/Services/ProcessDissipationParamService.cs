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
    public class ProcessDissipationParamService : Service<ProcessDissipationParam>, IProcessDissipationParamService
    {
        [Inject]
        private readonly IRepository<ProcessDissipationParam> _repository;


        public ProcessDissipationParamService(IRepository<ProcessDissipationParam> repository)
            : base(repository)
        {
            _repository = repository;
        }
    
    }
}
