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
            return repository.Queryable()
                .Where(pf => pf.ProcessID == processId)
                .Where(pf => pf.FlowID == flowId)
                .Where(pf => pf.DirectionID != ex_directionId)
                .Select(pf => pf.Result).FirstOrDefault();
            /* *********
            var my_val = repository.Query(pf => pf.ProcessID == processId)
                .Select()
                .Where(pf => pf.FlowID == flowId)
                .Where(pf => pf.DirectionID != ex_directionId).FirstOrDefault();

                if (scenarioId != Scenario.MODEL_BASE_CASE_ID && pf.Flow.FlowTypeID == 2) // params only apply to elem flows
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

            //return my_val.Result;
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
        public static IEnumerable<ProcessFlow> GetProductFlows(this IRepositoryAsync<ProcessFlow> repository,
            int processId)
        {
            return repository.Queryable()
                .Where(pf => pf.ProcessID == processId)
                .Where(pf => pf.Flow.FlowTypeID == 1).ToList();
            //var Outflows = repository.Query(pf => pf.ProcessID == processId).Include(pf => pf.Flow).Select()
            //    .Where(pf => pf.Flow.FlowTypeID == 1).ToList();
            //return Outflows;

        }

        public static IEnumerable<InventoryModel> GetEmissions(this IRepositoryAsync<ProcessFlow> repository,
            int processId, int scenarioId)
        {
            if (scenarioId == Scenario.MODEL_BASE_CASE_ID)
                return repository.Queryable()
                    .Where(pf => pf.ProcessID == processId)
                    .Where(pf => pf.Flow.FlowTypeID == 2)
                    .Select(pf => new InventoryModel 
                    {
                        FlowID = pf.FlowID,
                        DirectionID = pf.DirectionID,
                        Result = pf.Result
                    });
            else
                return repository.Queryable()
                    .Where(pf => pf.ProcessID == processId)
                    .Where(pf => pf.Flow.FlowTypeID == 2)
                    .GroupJoin(repository.GetRepository<ProcessEmissionParam>().Queryable()
                                .Where(pep => pep.Param.ScenarioID == scenarioId),
                        pf => pf.ProcessFlowID,
                        pep => pep.ProcessFlowID,
                        (pf, pep) => new { baseFlows = pf, efParams = pep })
                    .SelectMany(s => s.efParams.DefaultIfEmpty(),
                        (s, efParams) => new InventoryModel
                        {
                            FlowID = s.baseFlows.FlowID,
                            DirectionID = s.baseFlows.DirectionID,
                            Result = s.baseFlows.Result,
                            Param = efParams == null ? null : new ParamInstance
                            {
                                ParamID = efParams.ParamID,
                                Value = efParams.Value
                            }
                        });

        }

        /*
        public static IEnumerable<InventoryModel> GetDissipation(this IRepositoryAsync<ProcessFlow> repository,
            int processId, int scenarioId)
        {
            if (scenarioId == Scenario.MODEL_BASE_CASE_ID)
                // no params
                return repository.Queryable()
                    .Where(pf => pf.ProcessID == processId)
        }

        */

        /*
        public static IEnumerable<InventoryModel> GetEmissionsOld(this IRepositoryAsync<ProcessFlow> repository,
            int processId, int scenarioId)
        {
            var inventory2 = repository.Queryable()
                                .Where(x => x.ProcessID == processId)
                                .Where(x => x.Flow.FlowTypeID == 2)
                                .GroupJoin(repository.GetRepository<ProcessEmissionParam>().Queryable() // Target table
                                    , pf => pf.ProcessFlowID
                                    , pep => pep.ProcessFlowID
                                    , (pf, pep) => new { processFlows = pf, processEmmissionParams = pep })
                                .SelectMany(s => s.processEmmissionParams.DefaultIfEmpty()
                                    , (s, processEmmissionParams) => new {
                                        FlowID = s.processFlows.FlowID,
                                        DirectionID = s.processFlows.DirectionID,
                                        ParamID = processEmmissionParams == null ? 0 : processEmmissionParams.ParamID,
                                        Result = s.processFlows.Result,
                                        ParamValue = processEmmissionParams == null ? 0 : processEmmissionParams.Value
                                    })
                                .GroupJoin(repository.GetRepository<Param>().Queryable().Where(p => p.ScenarioID == scenarioId) // Target table
                                    , pep => pep.ParamID
                                    , p => p.ParamID
                                    , (pep, p) => new { processEmmissionParams = pep, parameters = p })
                                .SelectMany(s => s.parameters.DefaultIfEmpty()
                                    , (s, parameters) => new
                                    {
                                        FlowID = s.processEmmissionParams.FlowID,
                                        DirectionID = s.processEmmissionParams.DirectionID,
                                        ParamID = parameters == null ? 0 : parameters.ParamID,
                                        Result = s.processEmmissionParams.Result,
                                        ParamValue = s.processEmmissionParams == null ? 0 : s.processEmmissionParams.ParamValue
                                    })
                //leave this where clause out for now as there are no records in ProcessEmissionParam table with which to join on the Param table
                //so this where clause will result in no records being returned
                //.Where(x => x.sp.ScenarioID == scenarioId)
                                .Select(inv => new InventoryModel
                                {
                                    FlowID = inv.FlowID,
                                    DirectionID = inv.DirectionID,
                                    Result = inv.Result,
                                    ParamValue = inv.ParamValue
                                });

            return inventory2;
        }

         * */
    }
}
