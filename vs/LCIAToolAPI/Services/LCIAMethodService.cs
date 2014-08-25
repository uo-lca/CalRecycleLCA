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
    public class LCIAMethodService : Service<LCIAMethod>, ILCIAMethodService
    {
        [Inject]
        private readonly IRepository<LCIAMethod> _repository;


        public LCIAMethodService(IRepository<LCIAMethod> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
