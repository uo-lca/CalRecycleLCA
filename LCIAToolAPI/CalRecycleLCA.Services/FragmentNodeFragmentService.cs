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
    public interface IFragmentNodeFragmentService : IService<FragmentNodeFragment>
    {
    }

    public class FragmentNodeFragmentService : Service<FragmentNodeFragment>, IFragmentNodeFragmentService
    {
        public FragmentNodeFragmentService(IRepositoryAsync<FragmentNodeFragment> repository)
            : base(repository)
        {
                    
        }
    }
}
