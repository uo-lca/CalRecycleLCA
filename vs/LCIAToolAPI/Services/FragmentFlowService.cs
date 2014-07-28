using LcaDataModel;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FragmentFlowService : Service<FragmentFlow>, IFragmentFlowService
    {
         private readonly IRepository<FragmentFlow> _repository;

         public FragmentFlowService(IRepository<FragmentFlow> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<FragmentFlow> GetFragmentFlows(int fragmentId)
        {
            return _repository.GetFragmentFlows(fragmentId);
        }
    }
}
