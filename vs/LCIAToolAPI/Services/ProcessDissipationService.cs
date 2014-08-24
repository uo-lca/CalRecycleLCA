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
    public class ProcessDissipationService : Service<ProcessDissipation>, IProcessDissipationService
    {
        [Inject]
        private readonly IRepository<ProcessDissipation> _repository;


        public ProcessDissipationService(IRepository<ProcessDissipation> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
