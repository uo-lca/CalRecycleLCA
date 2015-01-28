using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IFragmentService : IService<Fragment>
    {
        int Count();
    }

    public class FragmentService : Service<Fragment>, IFragmentService
    {
        private IRepositoryAsync<Fragment> _repository;
        public FragmentService(IRepositoryAsync<Fragment> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public int Count()
        {
            return _repository.Queryable().Count();
        }
    }
}

