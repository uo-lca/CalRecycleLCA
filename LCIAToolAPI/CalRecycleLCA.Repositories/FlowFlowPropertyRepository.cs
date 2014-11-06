﻿using LcaDataModel;
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
            int inFlowId, int flowPropertyId, int scenarioId = 0)
        {
            var my_val = repository.Query(ffp => ffp.FlowID == inFlowId)
                .Select()
                .Where(ffp => ffp.FlowPropertyID == flowPropertyId).FirstOrDefault();

            if (scenarioId != 0)
            {
                var fp_param = repository.GetRepository<FlowPropertyParam>()
                    .Queryable()
                    .Where(w => w.FlowFlowPropertyID == my_val.FlowFlowPropertyID)
                    .Where(w => w.Param.ScenarioID == scenarioId)
                    .FirstOrDefault();
            
                if (fp_param != null)
                    my_val.MeanValue = fp_param.Value;
            }

            return (double)my_val.MeanValue;
        }

        public static double? FlowConv(this IRepositoryAsync<FlowFlowProperty> repository,
            int refFlowId, int inFlowId, int scenarioId = 0)
        {
            double flow_conv = 1;
            int refProp = repository.GetRepository<Flow>().Query(f => f.FlowID == refFlowId)
                .Select().First().ReferenceFlowProperty;

            flow_conv = inFlowIn(repository, inFlowId, refProp, scenarioId);

            return flow_conv;
        }
    }
}
