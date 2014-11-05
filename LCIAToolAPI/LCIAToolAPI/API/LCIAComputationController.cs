
using Entities.Models;
using LcaDataModel;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CalRecycleLCA.Services;

namespace LCAToolAPI.API
{
    public class LCIAComputationController : ApiController
    {
        [Inject]
        private readonly ILCIAComputationV2 _lciaComputationV2;
        [Inject]
        private readonly IFragmentLCIAComputation _fragmentLCIAComputation;
        [Inject]
        private readonly ITestGenericService _testService;


        public LCIAComputationController(
            ILCIAComputationV2 lciaComputationV2, 
            IFragmentLCIAComputation fragmentLCIAComputation,
            ITestGenericService testGenericService)
        {

            if (lciaComputationV2 == null)
            {
                throw new ArgumentNullException("lciaComputationV2 is null");
            }

            _lciaComputationV2 = lciaComputationV2;

            if (fragmentLCIAComputation == null)
            {
                throw new ArgumentNullException("fragmentLCIAComputation is null");
            }

            _fragmentLCIAComputation = fragmentLCIAComputation;

            if (testGenericService == null)
            {
                throw new ArgumentNullException("testGenericService is null");
            }

            _testService = testGenericService;
        }

        //GET api/<controller>
        [Route("api/processes/{ProcessID}/scenarios/{scenarioID}/compute")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<LCIAModel> LCIACompute(int processId, int scenarioId)
        {
            return _lciaComputationV2.LCIACompute(processId, scenarioId);
        }

        [Route("api/processes/{ProcessID}/scenarios/{scenarioID}/inventory")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<InventoryModel> LCICompute(int processId, int scenarioId)
        {
            return _lciaComputationV2.ComputeProcessLCI(processId, scenarioId);
        }

        [Route("api/fragments/{FragmentID}/scenarios/{scenarioID}/compute")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public void LCIAFragmentCompute(int fragmentId, int scenarioId)
        {
            _fragmentLCIAComputation.FragmentLCIACompute(fragmentId, scenarioId);
        }

        // Diagnostic functions
        [Route("api/fragmentflows/{fragmentFlowID}/scenarios/{scenarioID}/node")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public FragmentNodeResource FindTerminus(int fragmentFlowID, int scenarioID)
        {
            return _testService.FindTerminus(fragmentFlowID, scenarioID);
        }

        [Route("api/fragmentflows/{fragmentFlowID}/scenarios/{scenarioID}/outflows")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<InventoryModel> GetDependencies(int fragmentFlowID, int scenarioID)
        {
            return _testService.GetDependencies(fragmentFlowID, scenarioID);
        }



       
    }
}
