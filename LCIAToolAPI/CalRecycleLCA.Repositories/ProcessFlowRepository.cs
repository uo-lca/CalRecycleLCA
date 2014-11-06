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
    public static class ProcessFlowRepository
    {
        public static double? FlowExchange(this IRepositoryAsync<ProcessFlow> repository,
					  int processId, int flowId, int ex_directionId)
        {
            var my_val = repository.Query(pf => pf.ProcessID == processId)
                .Select()
                .Where(pf => pf.FlowID == flowId)
		        .Where(pf => pf.DirectionID != ex_directionId).FirstOrDefault();

	    /* *********
            if (scenarioId != 0 && pf.Flow.FlowTypeID == 2) // params only apply to elem flows
            {
                var pf_param = repository.GetRepository<FlowPropertyParam>()
                    .Queryable()
                    .Where(w => w.FlowFlowPropertyID == my_val.FlowFlowPropertyID)
                    .Where(w => w.Param.ScenarioID == scenarioId)
                    .FirstOrDefault();
            
                if (fp_param != null)
                    my_val.MeanValue = fp_param.Value;
            }
	    ***** */

            return my_val.Result;
        }

        /// <summary>
        /// Given a process and a reference inflow, reports all other intermediate flows that result as dependencies.  
        /// ex_directionId should be provided from the perspective of the *parent* (i.e. as entered in the 
        /// FragmentFlow table).  
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="flowId"></param>
        /// <param name="ex_directionId"></param>
        /// <returns></returns>
        public static IEnumerable<InventoryModel> GetDependencies(this IRepositoryAsync<ProcessFlow> repository,
            int processId, int flowId, int ex_directionId)
        {
            int myDirectionId = 1;
            if (ex_directionId == 1)
                myDirectionId = 2;

            var Outflows = repository.Query(pf => pf.ProcessID == processId).Include(pf => pf.Flow).Select()
                .Where(pf => pf.Flow.FlowTypeID == 1).ToList();

            int refPfId = Outflows
                .Where(pf => pf.FlowID == flowId)
                .Where(pf => pf.DirectionID == myDirectionId)
                .First().ProcessFlowID;

            return Outflows.Where(o => o.ProcessFlowID != refPfId)
                .Select(a => new InventoryModel { 
                    FlowID = a.FlowID,
                    DirectionID = a.DirectionID,
                    Result = a.Result
                }).ToList();
        }
    }
}
