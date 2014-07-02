using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Repository;
using Ninject;

namespace Services
{
    public class FragmentService : Service<Fragment>, IFragmentService
    {
        [Inject]
        private readonly IRepository<Fragment> _repository;


        public FragmentService(IRepository<Fragment> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<Fragment> GetFragments()
        {
            return _repository.GetFragments();
        }

    }
}
