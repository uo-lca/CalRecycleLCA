using LcaDataModel;
using Repository.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Entities.Models;


namespace CalRecycleLCA.Repositories
{
    public static class FlowFlowPropertyRepository
    {
        public static double inFlowIn(this IRepositoryAsync<FlowFlowProperty> repository,
            int inFlowId, int flowPropertyId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            var my_val = repository.Query(ffp => ffp.FlowID == inFlowId)
                .Select()
                .Where(ffp => ffp.FlowPropertyID == flowPropertyId).FirstOrDefault();

            if (scenarioId != Scenario.MODEL_BASE_CASE_ID)
            {
                var fp_param = repository.GetRepository<FlowPropertyParam>()
                    .Queryable()
                    .Where(w => w.FlowFlowPropertyID == my_val.FlowFlowPropertyID)
                    .Where(w => w.Param.ScenarioID == scenarioId)
                    .FirstOrDefault();
            
                if (fp_param != null)
                    my_val.MeanValue = fp_param.Value;
            }

            return my_val.MeanValue;
        }

        public static double? FlowConv(this IRepositoryAsync<FlowFlowProperty> repository,
            int refFlowId, int inFlowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            double flow_conv = 1;
            int refProp = repository.GetRepository<Flow>().Query(f => f.FlowID == refFlowId)
                .Select().First().ReferenceFlowProperty;

            flow_conv = inFlowIn(repository, inFlowId, refProp, scenarioId);

            return flow_conv;
        }

        public static IEnumerable<FlowPropertyMagnitude> GetFlowPropertyMagnitudes(this IRepository<FlowFlowProperty> repository,
            int flowId, int scenarioId, double scale = 1.0)
        {
            return repository.Queryable().Where(fp => fp.FlowID == flowId)
                .GroupJoin(repository.GetRepository<FlowPropertyParam>().Queryable()
                    .Where(p => p.Param.ScenarioID == scenarioId),
                    fp => fp.FlowFlowPropertyID,
                    p => p.FlowFlowPropertyID,
                    (fp, p) => new { factors = fp, parameter = p })
                    .SelectMany(s => s.parameter.DefaultIfEmpty(),
                    (s, parameter) => new FlowPropertyMagnitude
                    {
                        FlowPropertyID = s.factors.FlowPropertyID,
                        Unit = s.factors.FlowProperty.UnitGroup.ReferenceUnit,
                        Magnitude = parameter == null ? scale * s.factors.MeanValue : scale * parameter.Value
                    });
        }
    }
}
