using Entities.Models;
using LcaDataModel;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LCIAComputationV2 : ILCIAComputationV2
    {
        [Inject]
        private readonly IProcessFlowService _processFlowService;
        [Inject]
        private readonly IProcessEmissionParamService _processEmissionParamService;
        [Inject]
        private readonly IScenarioParamService _scenarioParamService;
        [Inject]
        private readonly ILCIAMethodService _lciaMethodService;
        [Inject]
        private readonly IFlowService _flowService;
        [Inject]
        private readonly IFlowFlowPropertyService _flowFlowPropertyService;
        [Inject]
        private readonly IFlowPropertyParamService _flowPropertyParamService;
        [Inject]
        private readonly IFlowPropertyEmissionService _flowPropertyEmissionService;
        [Inject]
        private readonly IProcessDissipationService _processDissipationService;
        [Inject]
        private readonly IProcessDissipationParamService _processDissipationParamService;
        [Inject]
        private readonly ILCIAService _lciaService;
        [Inject]
        private readonly ICharacterizationParamService _characterizationParamService;

        public LCIAComputationV2(IProcessFlowService processFlowService,
            IProcessEmissionParamService processEmissionParamService,
            IScenarioParamService scenarioParamService,
            ILCIAMethodService lciaMethodService,
            IFlowService flowService,
            IFlowFlowPropertyService flowFlowPropertyService,
            IFlowPropertyParamService flowPropertyParamService,
            IFlowPropertyEmissionService flowPropertyEmissionService,
            IProcessDissipationService processDissipationService,
            IProcessDissipationParamService processDissipationParamService,
            ILCIAService lciaService,
            ICharacterizationParamService characterizationParamService)
        {
            if (processFlowService == null)
            {
                throw new ArgumentNullException("processFlowService is null");
            }
            _processFlowService = processFlowService;

            if (processEmissionParamService == null)
            {
                throw new ArgumentNullException("processEmissionParamService is null");
            }
            _processEmissionParamService = processEmissionParamService;

            if (scenarioParamService == null)
            {
                throw new ArgumentNullException("scenarioParamService is null");
            }
            _scenarioParamService = scenarioParamService;

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

            if (processDissipationService == null)
            {
                throw new ArgumentNullException("processDissipationService is null");
            }
            _processDissipationService = processDissipationService;

            if (processDissipationParamService == null)
            {
                throw new ArgumentNullException("processDissipationParamService is null");
            }
            _processDissipationParamService = processDissipationParamService;

            if (lciaService == null)
            {
                throw new ArgumentNullException("lciaService is null");
            }
            _lciaService = lciaService;

            if (characterizationParamService == null)
            {
                throw new ArgumentNullException("characterizationParamService is null");
            }
            _characterizationParamService = characterizationParamService;
        }

        public void GetLCIAMethodsForComputeLCIA()
        {
            var lciaMethods = _lciaMethodService.Query().Get();
            ComputeLCIA(1, lciaMethods, 1);
        }

        public void ComputeLCIA(int processId, IEnumerable<LCIAMethod> lciaMethods, int scenarioId=1)
        {
            var inventory = ComputeProcessLCI(processId, scenarioId);
            foreach (var lciaMethodItem in lciaMethods)
            {
                var lcia = ComputeProcessLCIA(inventory, lciaMethodItem, scenarioId);
               //lciaMethodItem.
                
            }
//            [2:42:51 PM] Brandon Kuczenski: ComputeLCIA( process_id, scenario_id, list_of_lcia_methods)
//{
//[2:44:31 PM] Brandon Kuczenski:   inventory = ComputeProcessLCI (process_id, scenario_id );
//  for j in list_of_lcia_methods do
//  {
//    lcia = ComputeProcessLCIA ( inventory, scenario_id, list_of_lcia_methods[j])
            //this doesn't work.  There is no "score" in LCIAMethod
//    score[j] = sum (lcia.Score)
//  }
//}
        }

        //inventory in pseudocode
        public IEnumerable<InventoryModel> ComputeProcessLCI(int processId, int scenarioId)
        {
             // returns a list of flows: FlowID, DirectionID, Result
  // Param types: ProcessEmissionParam
  // FlowPropertyParam + ProcessDissipationParam
            var inventory = _processFlowService.Query().Get()
                .Where(x => x.ProcessID == processId)
          .GroupJoin(_processEmissionParamService.Query().Get() // Target table
      , pf => pf.ProcessFlowID
      , pep => pep.ProcessFlowID
      , (pf, pep) => new { processFlows = pf, processEmmissionParams = pep })
      .SelectMany(s => s.processEmmissionParams.DefaultIfEmpty()
      , (s, processEmmissionParams) => new
      {

            FlowID = s.processFlows.FlowID,
                              DirectionID = s.processFlows.DirectionID,
                              ParamID = processEmmissionParams.ParamID,
                              Result = s.processFlows.Result,
                              ParamValue = processEmmissionParams.Value
      })
     .Join(_scenarioParamService.Query().Get(), pfep => pfep.ParamID, sp => sp.ParamID, (pfep, sp) => new { pfep, sp })
    .Where(x => x.sp.ScenarioID == scenarioId)
      .Select(inv => new InventoryModel
                          {
                              FlowID = inv.pfep.FlowID,
                              DirectionID = inv.pfep.DirectionID, 
                              Result = inv.pfep.Result,
                              ParamValue = inv.pfep.ParamValue
                          });
           

            foreach (var item in inventory)
            {
                if (item.ParamValue != null)
                {
                    item.Result = item.ParamValue;
                }
            }

            return inventory;

        }

        public IEnumerable<ProcessFlow> ComputeProcessDissipation(int processId, int scenarioId)
        {

             // Leave this as a stub for now - Brandon informed me on 8/22 that this would be changing signicantly in his pseudocode and to wait on it.
            throw new NotImplementedException();


    //        var inflows = _processFlowService.Query().Get()
    //            .Join(_flowService.Query().Get(), pf => pf.FlowID, f => f.FlowID, (pf, f) => new { pf, f })
    //.Where(x => x.pf.DirectionID == 1)  // Input
    //.Where(x => x.pf.ProcessID == 1)
    //.Where(x => x.f.FlowTypeID == 1).ToList(); //IntemediateFlow


    //        var dissipation = inflows
    //             .Join(_flowFlowPropertyService.Query().Get(), x => x.f.FlowID, ffp => ffp.FlowID, (i, ffp) => new { i, ffp })
    //             .Join(_flowPropertyEmissionService.Query().Get(), x => x.ffp.FlowID, ffp => ffp.FlowID, (ffp, fpe) => new { ffp, fpe})
    //             .Join(_processFlowService.Query().Get(), x => x.fpe.FlowID, pf => pf.FlowID, (fpe, pf) => new { fpe, pf})
    //             .Join(_processDissipationService.Query().Get(), x => x.pf.ProcessFlowID, pd => pd.ProcessFlowID, (pf, pd) => new { pf, pd})
    //             .GroupJoin(_processDissipationParamService.Query().Get() // Target table
    //  , pd => pd.pd.ProcessDissipationID
    //  , pdp => pdp.ProcessDissipationID
    //  , (pd, pdp) => new { processDissipation = pd, processDissipationParams = pdp })
    //  .SelectMany(s => s.processDissipationParams.DefaultIfEmpty()
    //  , (s, processDissipationParams) => new
    //  {

    //        ProcessDissipationID = s.processDissipation.pd.ProcessDissipationID,
    //        Value = processDissipationParams.Param
    //  })




            

        }

        public IEnumerable<LCIAModel> ComputeProcessLCIA(IEnumerable<InventoryModel> inventory, LCIAMethod lciaMethodItem, int scenarioId)
        {
            //// Leave this as a stub for now
            //throw new NotImplementedException();

            var lcias = inventory
                .Join(_lciaService.Query().Get(), i => i.FlowID, l => l.FlowID, (i, l) => new { i, l })
                .Join(_lciaMethodService.Query().Get(), l => l.l.LCIAMethodID, lm => lm.LCIAMethodID, (l, lm) => new { l, lm })
                 .GroupJoin(_characterizationParamService.Query().Get() // Target table
      , l => l.l.l.LCIAID
      , cp => cp.LCAID
      , (l, cp) => new { lcias = l, characterizationParams = cp })
      .SelectMany(s => s.characterizationParams.DefaultIfEmpty()
      , (s, characterizationParams) => new
      {
          FlowID= s.lcias.l.i.FlowID,
          DirectionID = s.lcias.l.i.DirectionID,
          Quantity = s.lcias.l.i.Result,
          LCIAID = characterizationParams.LCAID,
          Value = characterizationParams.Value,
          ParamID = characterizationParams.ParamID,
          LCIAMethodID = s.lcias.lm.LCIAMethodID,
          Geography = s.lcias.l.l.Geography,
          Factor = s.lcias.l.l.Factor
      })
      .Join(_scenarioParamService.Query().Get(), cp => cp.ParamID, sp => sp.ParamID, (cp, sp) => new { cp, sp })
      .Select(lcparam => new LCIAModel
        {
            LCIAID = lcparam.cp.LCIAID,
            FlowID = lcparam.cp.FlowID,
            Value = lcparam.cp.Value,
            ParamID = lcparam.cp.ParamID,
            ScenarioID = lcparam.sp.ScenarioID,
            LCIAMethodID = lcparam.cp.LCIAMethodID,
            DirectionID = lcparam.cp.DirectionID,
            Geography = lcparam.cp.Geography,
            Result = lcparam.cp.Quantity,
            LCParamValue =lcparam.cp.Value,
            LCIAFactor = lcparam.cp.Factor
        })
        .Where(x => x.ScenarioID == scenarioId)
        .Where(x => x.LCIAMethodID == lciaMethodItem.LCIAMethodID)
        .Where(x => x.DirectionID == inventory.Select(i => i.DirectionID).FirstOrDefault())
        .Where(x => x.Geography == null);

            foreach (var item in lcias)
            {
                double? factor;
                double? result;
        //        inventory.Result * (lcparam.Value == NULL 
        //    ? LCIA.Factor : lcparam.Value) AS Factor,
        //Quantity * Factor AS Result
                if (item.LCParamValue != null)
                {
                    factor = item.Result * item.LCParamValue;
                    result = item.Result * factor;
                }
                else
                {
                    factor = item.Result * item.LCIAFactor;
                    result = item.Result * factor;
                }

            }

            return lcias;



        }
    }
}
