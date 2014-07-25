using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Repository
{
    public static class FragmentFlowRepository
    {
        public static IEnumerable<FragmentFlow> GetFragmentFlows(this IRepository<FragmentFlow> fragmentFlowRepository, int fragmentId)
        {
            var fragmentFlows =
       fragmentFlowRepository
            .Query()
            .Include(i => i.FragmentNodeFragments)
            .Include(i => i.FragmentNodeProcesses)
            .Include(i => i.NodeCaches)
            .Include(i => i.Flow.FlowFlowProperties)
                //.OrderBy(q => q
                //    .OrderBy(c => c.FlowID)
                //    .ThenBy(c => c.Name))
            .Filter(q => q.FragmentID == fragmentId)
                //.GetPage();
            .Get();

            return fragmentFlows;
        }
    }
}
