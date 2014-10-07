using LcaDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using Repository.Pattern.Repositories;

namespace CalRecycleLCA.Repository
{
    public static class FlowPropertyParamRepository
    {
        public static IEnumerable<FlowPropertyParam> GetFlowPropertyParams(this IRepositoryAsync<FlowPropertyParam> flowPropertyParamRepository, int scenarioId)
        {
            var flowPropertyParams = flowPropertyParamRepository.Queryable();
            return flowPropertyParams;

        }
    }
}
