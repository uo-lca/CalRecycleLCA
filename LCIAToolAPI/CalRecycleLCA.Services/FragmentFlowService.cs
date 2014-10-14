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
    public interface IFragmentFlowService : IService<FragmentFlow>
    {
    }

    public class FragmentFlowService : Service<FragmentFlow>, IFragmentFlowService
    {
        public FragmentFlowService(IRepositoryAsync<FragmentFlow> repository)
            : base(repository)
        {

        }
    }
}
