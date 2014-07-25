using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Repository
{
    public static class FlowPropertyRepository
    {

        /// <summary>
        /// Query for flow properties related to fragment. Include UnitGroup object.
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>query result</returns>
        public static IEnumerable<FlowProperty> GetFlowPropertiesByFragment(this IRepository<FlowProperty> FlowPropertyRepository, int fragmentID) {
            return FlowPropertyRepository.GetRepository<FlowProperty>()
                .Query()
                .Include(fp => fp.UnitGroup.UnitConversion)
                .Filter(fp => fp.Flows.Any(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentID)))
                .Get();
        }
    }
}
