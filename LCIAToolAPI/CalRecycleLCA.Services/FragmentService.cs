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
    }

    public class FragmentService : Service<Fragment>, IFragmentService
    {
        public FragmentService(IRepositoryAsync<Fragment> repository)
            : base(repository)
        {

        }
    }
}

