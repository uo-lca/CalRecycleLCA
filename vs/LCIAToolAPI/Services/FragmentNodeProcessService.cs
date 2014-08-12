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
    public class FragmentNodeProcessService : Service<FragmentNodeProcess>, IFragmentNodeProcessService
    {
         [Inject]
        private readonly IRepository<FragmentNodeProcess> _repository;


         public FragmentNodeProcessService(IRepository<FragmentNodeProcess> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
