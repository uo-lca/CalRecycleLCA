using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Repository
{
    public static class FlowRepository
    {
        public static IEnumerable<Flow> GetFlowsByFragment(this IRepository<Flow> flowRepository, int fragmentId)
        {
            var flows =
       flowRepository
            .Query()
            .OrderBy(q => q
                .OrderBy(c => c.FlowID)
                .ThenBy(c => c.Name))
            .Filter(q => q.FlowID == fragmentId)
            .GetPage();

            return flows;
        }
    }
}
