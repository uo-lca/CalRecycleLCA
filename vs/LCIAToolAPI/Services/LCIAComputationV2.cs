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
        private readonly IService<ProcessFlow> _processFlowService;
        [Inject]
        private readonly IService<ProcessEmissionParam> _processEmissionParamService;
        [Inject]
        private readonly IService<LCIAMethod> _lciaMethodService;
        [Inject]
        private readonly IService<Flow> _flowService;
        [Inject]
        private readonly IService<FlowFlowProperty> _flowFlowPropertyService;
        [Inject]
        private readonly IService<FlowPropertyParam> _flowPropertyParamService;
        [Inject]
        private readonly IService<FlowPropertyEmission> _flowPropertyEmissionService;
        [Inject]
        private readonly IService<ProcessDissipation> _processDissipationService;
        [Inject]
        private readonly IService<ProcessDissipationParam> _processDissipationParamService;
        [Inject]
        private readonly IService<LCIA> _lciaService;
        [Inject]
        private readonly IService<CharacterizationParam> _characterizationParamService;
        [Inject]
        private readonly IService<Param> _paramService;

        public LCIAComputationV2(IService<ProcessFlow> processFlowService,
            IService<ProcessEmissionParam> processEmissionParamService,
            IService<LCIAMethod> lciaMethodService,
            IService<Flow> flowService,
            IService<FlowFlowProperty> flowFlowPropertyService,
            IService<FlowPropertyParam> flowPropertyParamService,
            IService<FlowPropertyEmission> flowPropertyEmissionService,
            IService<ProcessDissipation> processDissipationService,
            IService<ProcessDissipationParam> processDissipationParamService,
            IService<LCIA> lciaService,
            IService<CharacterizationParam> characterizationParamService,
            IService<Param> paramService)
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

            if (paramService == null)
            {
                throw new ArgumentNullException("paramService is null");
            }
            _paramService = paramService;
        }

        public double Compute(int processId, int scenarioId)
        {
            var lciaMethods = _lciaMethodService.Query().Get();
            var result = ProcessLCIA(processId, lciaMethods, scenarioId);
            return result;

        }

        public double ProcessLCIA(int? processId, IEnumerable<LCIAMethod> lciaMethods, int? scenarioId = 1)
        {
            var inventory = ComputeProcessLCI(processId, scenarioId);
            IEnumerable<LCIAModel> lcias=null;
            IEnumerable<LCIAModel> scores = null;

            foreach (var lciaMethodItem in lciaMethods.ToList())
            {
               
               lcias= ComputeProcessLCIA(inventory, lciaMethodItem, scenarioId);

               if (lcias.Count() != 0)
               {
                   scores = lcias.ToList()
                        .GroupBy(t => new
                     {
                         t.LCIAMethodID,
                         t.Result,
                         t.DirectionID,
                         t.FlowID
                     })
                     .Select(group => new LCIAModel
                     {
                         Result = group.Key.Result,
                         DirectionID = group.Key.DirectionID,
                         FlowID = group.Key.FlowID,
                         LCIAMethodID = group.Key.LCIAMethodID
                     });
               }

            }
            double total = Convert.ToDouble(scores.Sum(x => x.Result));

            return total;
        }

        //inventory in pseudocode
        public IEnumerable<InventoryModel> ComputeProcessLCI(int? processId, int? scenarioId)
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
          ParamID = processEmmissionParams == null ? 0 : processEmmissionParams.ParamID,
          Result = s.processFlows.Result,
          ParamValue = processEmmissionParams == null ? 0 : processEmmissionParams.Value
      })
        .GroupJoin(_paramService.Query().Get() // Target table
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

        }

        public IEnumerable<LCIAModel> ComputeProcessLCIA(IEnumerable<InventoryModel> inventory, LCIAMethod lciaMethodItem, int? scenarioId)
        {
            var lcias = inventory
                .Join(_lciaService.Query().Get(), i => i.FlowID, l => l.FlowID, (i, l) => new { i, l })
                .Where(x => x.l.FlowID != null)
                .Join(_lciaMethodService.Query().Get(), l => l.l.LCIAMethodID, lm => lm.LCIAMethodID, (l, lm) => new { l, lm })
                 .Where(x => x.l.l.LCIAMethodID == lciaMethodItem.LCIAMethodID)
                 .GroupJoin(_characterizationParamService.Query().Get() // Target table
      , l => l.l.l.LCIAID
      , cp => cp.LCAID
      , (l, cp) => new { lcias = l, characterizationParams = cp })
      .SelectMany(s => s.characterizationParams.DefaultIfEmpty()
      , (s, characterizationParams) => new
      {
          FlowID = s.lcias.l.i.FlowID,
          DirectionID = s.lcias.l.i.DirectionID,
          Quantity = s.lcias.l.i.Result,
          LCIAID = characterizationParams == null ? 0 : characterizationParams.LCAID,
          Value = characterizationParams == null ? 0 : characterizationParams.Value,
          ParamID = characterizationParams == null ? 0 : characterizationParams.ParamID,
          LCIAMethodID = s.lcias.lm.LCIAMethodID,
          Geography = s.lcias.l.l.Geography,
          Factor = s.lcias.l.l.Factor
      })
      .GroupJoin(_paramService.Query().Get() // Target table
      , cp => cp.ParamID
      , p => p.ParamID
      , (cp, p) => new { characterizationParams = cp, parameters = p })
      .SelectMany(s => s.parameters.DefaultIfEmpty()
      , (s, parameters) => new LCIAModel
      {
          FlowID = s.characterizationParams.FlowID,
          DirectionID = s.characterizationParams.DirectionID,
          Result = s.characterizationParams.Quantity,
          LCIAID = s.characterizationParams.LCIAID,
          LCParamValue = s.characterizationParams.Value,
          ParamID = parameters == null ? 0 : parameters.ParamID,
          LCIAMethodID = s.characterizationParams.LCIAMethodID,
          Geography = s.characterizationParams.Geography,
          LCIAFactor = s.characterizationParams.Factor,
          ScenarioID = parameters == null ? 0 : parameters.ScenarioID
      }).ToList();
                //leave this where clause out for now as there are no records in CharacterizationParam table with which to join on the Param table
                //so this where clause will result in no records being returned
                //.Where(x => x.ScenarioID == scenarioId)
        //.Where(x => x.DirectionID == inventory.Select(i => i.DirectionID).FirstOrDefault());
        //.Where(x => x.Geography == null);

            double? computationResult;
            foreach (var item in lcias)
            {
                if (item.LCParamValue == 0)
                {
                    computationResult = (item.Result * item.LCIAFactor);
                }
                else
                {
                    computationResult = (item.Result * item.LCParamValue);
                }

                item.ComputationResult = computationResult;

            }

            return lcias;



        }
    }
}
