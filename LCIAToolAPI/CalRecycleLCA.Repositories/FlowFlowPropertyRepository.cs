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
        private static double? inFlowIn(this IRepository<FlowFlowProperty> repository,
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

            return (my_val == null) ? null : (double?)my_val.MeanValue;
        }

        public static double? FlowConv(this IRepository<FlowFlowProperty> repository,
            int refFlowId, int inFlowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            double? flow_conv = 1;
            int refProp = repository.GetRepository<Flow>().Query(f => f.FlowID == refFlowId)
                .Select().First().ReferenceFlowProperty;

            flow_conv = inFlowIn(repository, inFlowId, refProp, scenarioId);

            return flow_conv;
        }

        public static IEnumerable<FlowPropertyMagnitude> GetFlowPropertyMagnitudes(this IRepository<FlowFlowProperty> repository,
            int flowId, int scenarioId, double scale = 1.0)
        {
            int reffp = repository.GetRepository<Flow>().Queryable().Where(f => f.FlowID == flowId).Select(f => f.ReferenceFlowProperty).First();
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
                        //Unit = s.factors.FlowProperty.UnitGroup.ReferenceUnit,
                        Magnitude = parameter == null ? scale * s.factors.MeanValue : scale * parameter.Value
                    })
                .Union( // CompositionData manifest outwardly as FlowPropertyMagnitudes
                    repository.GetRepository<CompositionData>().Queryable().Where(cd => cd.CompositionModel.FlowID == flowId)
                    .GroupJoin(repository.GetRepository<CompositionParam>().Queryable()
                    .Where(p => p.Param.ScenarioID == scenarioId),
                    cd => cd.CompositionDataID,
                    cp => cp.CompositionDataID,
                    (cd,cp) => new { compositions = cd, parameter = cp})
                    .SelectMany(s => s.parameter.DefaultIfEmpty(),
                    (s,parameter) => new FlowPropertyMagnitude
                    {
                        FlowPropertyID = s.compositions.FlowPropertyID,
                        //Unit = s.compositions.FlowProperty.UnitGroup.ReferenceUnit,
                        // should we multiply by scale here?
                        Magnitude = parameter == null ? scale * s.compositions.Value : scale * parameter.Value
                    })
                ).OrderBy(m => m.FlowPropertyID != reffp);
        }
    }
}
