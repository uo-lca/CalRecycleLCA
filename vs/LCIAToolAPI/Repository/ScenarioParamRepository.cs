using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Repository
{
    public static class ScenarioParamRepository
    {
        public static IEnumerable<DependencyParam> GetDependencyParams(this IRepository<ScenarioParam> scenarioParamRepository, int scenarioId)
        {
            var scenarioParams = scenarioParamRepository.GetRepository<ScenarioParam>().Queryable().Where(sp => sp.ScenarioID == scenarioId);
            var param = scenarioParamRepository.GetRepository<Param>().Queryable();
            var dependencyParams = scenarioParamRepository.GetRepository<DependencyParam>().Queryable();

            var query = from sp in scenarioParams
                        join p in param on sp.ParamID equals p.ParamID
                        join dp in dependencyParams on p.ParamID equals dp.ParamID
                        select new DependencyParam
                        {
                            FragmentFlowID = dp.FragmentFlowID,
                            ParamID = sp.ParamID,
                            Value = dp.Value
                        };

            return query.AsEnumerable();


        }
    }
}
