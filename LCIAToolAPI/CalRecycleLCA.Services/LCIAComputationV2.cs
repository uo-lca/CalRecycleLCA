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
        //[Inject]
        //private readonly IFlowService _flowService;
        //[Inject]
        //private readonly IFlowFlowPropertyService _flowFlowPropertyService;
        //[Inject]
        //private readonly IFlowPropertyParamService _flowPropertyParamService;
        //[Inject]
        //private readonly IFlowPropertyEmissionService _flowPropertyEmissionService;
        //[Inject]
        //private readonly IProcessDissipationService _processDissipationService;
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
            //IFlowService flowService,
            //IFlowFlowPropertyService flowFlowPropertyService,
            //IFlowPropertyParamService flowPropertyParamService,
            //IFlowPropertyEmissionService flowPropertyEmissionService,
            //IProcessDissipationService processDissipationService,
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
            /* ***********
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

        public IEnumerable<LCIAResult> ProcessLCIA(int? processId, IEnumerable<int> lciaMethods, int? scenarioId)
        {
            var inventory = ComputeProcessLCI(processId, scenarioId);
            //IEnumerable<LCIAModel> lcias=null;
            List<LCIAResult> lciaResults = new List<LCIAResult>();
            foreach (var lciaMethodId in lciaMethods.ToList())
            {
                
                var lcias= ComputeProcessLCIA(inventory, lciaMethodId, scenarioId);
                lciaResults.Add(lcias);
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

        //inventory in pseudocode
        public IEnumerable<InventoryModel> ComputeProcessLCI(int? processId, int? scenarioId)
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
            return _processFlowService.GetEmissions((int)processId, (int)scenarioId);
        }

        public IEnumerable<ProcessFlow> ComputeProcessDissipation(int processId, int scenarioId)
        {
            // return _processFlowService.GetDissipation(processId, )
            // Need fix for #80
            throw new NotImplementedException();

        }

        private LCIAResult ComputeProcessLCIA(IEnumerable<InventoryModel> inventory, int lciaMethodId, int? scenarioId)
        {
            // var sw = Stopwatch.StartNew();
            LCIAResult lcias;
            if (scenarioId == null)
                lcias = _lciaService.ComputeLCIA(inventory, lciaMethodId);
            else
                lcias = _lciaService.ComputeLCIA(inventory, lciaMethodId, (int)scenarioId);

            // var t = sw.ElapsedMilliseconds;
            // sw.Stop();

            return lcias;



        }
    }
}
