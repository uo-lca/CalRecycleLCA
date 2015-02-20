using Entities.Models;
using LcaDataModel;
using Ninject;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public class LCIAComputationV2 : ILCIAComputationV2
    {
        [Inject]
        private readonly IProcessFlowService _processFlowService;
        //[Inject]
        //private readonly IProcessEmissionParamService _processEmissionParamService;
        [Inject]
        private readonly ILCIAMethodService _lciaMethodService;
        [Inject]
        private readonly IFlowService _flowService;
        //[Inject]
        //private readonly IFlowFlowPropertyService _flowFlowPropertyService;
        //[Inject]
        //private readonly IFlowPropertyParamService _flowPropertyParamService;
        //[Inject]
        //private readonly IFlowPropertyEmissionService _flowPropertyEmissionService;
        [Inject]
        private readonly IProcessDissipationService _processDissipationService;
        //[Inject]
        //private readonly IProcessDissipationParamService _processDissipationParamService;
        [Inject]
        private readonly ILCIAService _lciaService;
        //[Inject]
        //private readonly ICharacterizationParamService _characterizationParamService;
        //[Inject]
        //private readonly IParamService _paramService;

        public LCIAComputationV2(IProcessFlowService processFlowService,
            //IProcessEmissionParamService processEmissionParamService,
            ILCIAMethodService lciaMethodService,
            IFlowService flowService,
            //IFlowFlowPropertyService flowFlowPropertyService,
            //IFlowPropertyParamService flowPropertyParamService,
            //IFlowPropertyEmissionService flowPropertyEmissionService,
            IProcessDissipationService processDissipationService,
            //IProcessDissipationParamService processDissipationParamService,
            ILCIAService lciaService)
            //ICharacterizationParamService characterizationParamService,
            //IParamService paramService)
        {
            if (processFlowService == null)
            {
                throw new ArgumentNullException("processFlowService is null");
            }
            _processFlowService = processFlowService;
            /* ***********
            if (processEmissionParamService == null)
            {
                throw new ArgumentNullException("processEmissionParamService is null");
            }
            _processEmissionParamService = processEmissionParamService;
            *********** */

            if (lciaMethodService == null)
            {
                throw new ArgumentNullException("lciaMethodService is null");
            }
            _lciaMethodService = lciaMethodService;
            if (flowService == null)
            {
                throw new ArgumentNullException("flowService is null");
            }
            _flowService = flowService;

            /* ***********
            if (flowFlowPropertyService == null)
            {
                throw new ArgumentNullException("flowFlowPropertyService is null");
            }
            _flowFlowPropertyService = flowFlowPropertyService;

            if (flowPropertyParamService == null)
            {
                throw new ArgumentNullException("flowPropertyParamService is null");
            }
            _flowPropertyParamService = flowPropertyParamService;

            if (flowPropertyEmissionService == null)
            {
                throw new ArgumentNullException("flowPropertyEmissionService is null");
            }
            _flowPropertyEmissionService = flowPropertyEmissionService;
            ************* */

            if (processDissipationService == null)
            {
                throw new ArgumentNullException("processDissipationService is null");
            }
            _processDissipationService = processDissipationService;
            
            /*
            if (processDissipationParamService == null)
            {
                throw new ArgumentNullException("processDissipationParamService is null");
            }
            _processDissipationParamService = processDissipationParamService;
            ************* */

            if (lciaService == null)
            {
                throw new ArgumentNullException("lciaService is null");
            }
            _lciaService = lciaService;
            /* ************
            if (characterizationParamService == null)
            {
                throw new ArgumentNullException("characterizationParamService is null");
            }
            _characterizationParamService = characterizationParamService;

            if (paramService == null)
            {
                throw new ArgumentNullException("paramService is null");
            }
            _paramService = paramService;
            ************* */
        }


         /// <summary>
         /// Wrapper to specify all LCIA methods
         /// </summary>
         /// <param name="processId"></param>
         /// <param name="scenarioId"></param>
         /// <returns></returns>
        public IEnumerable<LCIAResult> LCIACompute(int processId, int scenarioId)
        {
            //var lciaMethods = from u in _lciaService.Queryable().AsEnumerable()
            //            select new LCIAModel
            //            {
            //                LCIAMethodID=u.LCIAMethodID
            //            };

            //return lciaMethods;

            var lciaMethods = _lciaMethodService.QueryActiveMethods();

            var result = ProcessLCIA(processId, lciaMethods, scenarioId);
            return result;

        }

        public IEnumerable<LCIAResult> ProcessLCIA(int processId, IEnumerable<int> lciaMethods, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            var inventory = ComputeProcessEmissions(processId, scenarioId);

            //var dissipation = new IEnumerable<InventoryModel>();
            var dissipation = ComputeProcessDissipation(processId, scenarioId);
            var diss_flows = new HashSet<int>(dissipation.Select(q => q.FlowID));

            //IEnumerable<LCIAModel> lcias=null;
            List<LCIAResult> lciaResults = new List<LCIAResult>();
            foreach (var lciaMethodId in lciaMethods.ToList())
            {
                var diss_lcias = new List<LCIAModel>();
                if (_processDissipationService.HasDissipation(processId))
                    diss_lcias = _lciaService.ComputeLCIADiss(dissipation, lciaMethodId, scenarioId); 

                var lcias = _lciaService.ComputeLCIA(inventory, lciaMethodId, scenarioId); 

                lcias.RemoveAll(k => k.DirectionID == (int)DirectionEnum.Output && diss_flows.Contains(k.FlowID));
                lcias.AddRange(diss_lcias);

                lciaResults.Add(new LCIAResult()
                {
                    LCIAMethodID = lciaMethodId,
                    ScenarioID = scenarioId,
                    LCIADetail = lcias
                });
                /*
                if (lcias.Count() == 0)
                {
                    lciaResults.Add(new LCIAModel()
                    {
                        LCIAMethodID = lciaMethodId,
                        Result = 0.0
                    });
                }
                else
                {
                   //get the sum of all the lcia scores in the lciamethoditem.
                   total = Convert.ToDouble(lcias.Sum(x => x.Result));
                   //direction = Convert.ToInt32(scores.Select(x => x.DirectionID).FirstOrDefault());

                   //add the sum of the scores to a list for each lciamethoditem
                   lciaResults.Add(new LCIAModel()
                   {
                       LCIAMethodID = lciaMethodId,
                       Result = total
                       //NodeLCIAResults = lcias
                   });
                   
               }
                */

            }
            return lciaResults;
        }

        public List<ProcessFlowResource> ComputeProcessLCI(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // there has to be code duplication here because we want the inventories to be unenumerated for LCIA, but enumerated here
            var flows = _processFlowService.GetProductFlows(processId).ToList();
            var inventory = ComputeProcessEmissions(processId, scenarioId).ToList();

            //var dissipation = new IEnumerable<InventoryModel>();
            var dissipation = ComputeProcessDissipation(processId, scenarioId).ToList();
            var diss_flows = new HashSet<int>(dissipation.Select(q => q.FlowID));

            inventory.RemoveAll(k => k.DirectionID == (int)DirectionEnum.Output && diss_flows.Contains(k.FlowID));

            flows.AddRange(dissipation);
            flows.AddRange(inventory);
            
            return flows.Select(k => new ProcessFlowResource()
                {
                    Flow = _flowService.GetFlow(k.FlowID).First(),
                    Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)k.DirectionID),
                    // VarName = omitted,
                    Content = k.Composition,
                    Dissipation = k.Dissipation,
                    Quantity = k.Result == null 
                        ? (double)k.Composition * (double)k.Dissipation
                        : (double)k.Result,
                    STDev = k.StDev == null ? 0.0 : (double)k.StDev
                }).ToList();
        }

        //inventory in pseudocode
        public IEnumerable<InventoryModel> ComputeProcessEmissions(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // returns a list of flows: FlowID, DirectionID, Result
            // Param types: ProcessEmissionParam
            // for DB optimization purposes, composition/dissipation emissions are computed separately
            // (join with LCIA happens on DB end and not in code)

            //var sw = Stopwatch.StartNew();
            // *************
            // repository.GetEmissionsOld
            // var Inv = _processFlowService.GetEmissionsOld((int)processId, (int)scenarioId);

            // *************
            // repository.GetEmissions -- eliminate GroupJoin(Param,...)
            return _processFlowService.GetEmissions(processId, scenarioId);
        }

        public IEnumerable<InventoryModel> ComputeProcessDissipation(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // return _processFlowService.GetDissipation(processId, )
            // Need fix for #80
            return _processDissipationService.GetDissipation(processId, scenarioId);
        }
    }
}
