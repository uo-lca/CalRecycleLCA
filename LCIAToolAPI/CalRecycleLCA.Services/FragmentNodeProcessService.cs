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
    public interface IFragmentNodeProcessService : IService<FragmentNodeProcess>
    {
    }

    public class FragmentNodeProcessService : Service<FragmentNodeProcess>, IFragmentNodeProcessService
    {
        public FragmentNodeProcessService(IRepositoryAsync<FragmentNodeProcess> repository)
            : base(repository)
        {
                    
        }
    }
}
