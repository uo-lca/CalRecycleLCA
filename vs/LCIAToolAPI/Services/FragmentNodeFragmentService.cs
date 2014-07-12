using Data;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FragmentNodeFragmentService : Service<FragmentNodeFragment>, IFragmentNodeFragmentService
    {
         [Inject]
        private readonly IRepository<FragmentNodeFragment> _repository;


         public FragmentNodeFragmentService(IRepository<FragmentNodeFragment> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
