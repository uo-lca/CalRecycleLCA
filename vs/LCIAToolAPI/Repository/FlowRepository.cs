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
        public static IEnumerable<Flow> GetFlows(this IRepository<Flow> flowRepository) {
            return flowRepository.GetRepository<Flow>().Queryable();
        }

        /// <summary>
        /// Query for flows related to fragment
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>query result</returns>
        public static IEnumerable<Flow> GetFlowsByFragment(this IRepository<Flow> flowRepository, int fragmentID) {
            return flowRepository.GetRepository<Flow>()
                .Query()
                .Filter(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentID))
                .Get();
        }
    }
}
