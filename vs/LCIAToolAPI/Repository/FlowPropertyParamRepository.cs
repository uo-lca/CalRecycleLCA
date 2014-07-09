using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public static class FlowPropertyParamRepository
    {
        public static IEnumerable<FlowPropertyParam> GetFlowPropertyParams(this IRepository<FlowPropertyParam> flowPropertyParamRepository, int scenarioId)
        {
            var flowPropertyParams = flowPropertyParamRepository.GetRepository<FlowPropertyParam>().Queryable();
            return flowPropertyParams;

        }
    }
}
