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
    public class LCIAService : Service<LCIA>, ILCIAService
    {
        [Inject]
        private readonly IRepository<LCIA> _repository;


        public LCIAService(IRepository<LCIA> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
