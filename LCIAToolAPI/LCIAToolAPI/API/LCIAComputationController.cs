﻿
using Entities.Models;
using LcaDataModel;
using Ninject;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CalRecycleLCA.Services;

namespace LCAToolAPI.API
{
    /// <summary>
    /// Internal API routes for testing LCIA computation and ScoreCache control
    /// </summary>
    public class LCIAComputationController : ApiController
    {
        [Inject]
        private readonly ILCIAComputationV2 _lciaComputationV2;
        [Inject]
        private readonly IFragmentLCIAComputation _fragmentLCIAComputation;
        //[Inject]
        //private readonly ITestGenericService _testService;

        /// <summary>
        /// Constructor for LCIA computation diagnostic controller.  Creates computation objects 
        /// via dependency injection.
        /// </summary>
        /// <param name="lciaComputationV2">via dependency injection</param>
        /// <param name="fragmentLCIAComputation">via dependency injection</param>
        public LCIAComputationController(
            ILCIAComputationV2 lciaComputationV2, 
            IFragmentLCIAComputation fragmentLCIAComputation)
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

            /*
            if (testGenericService == null)
            {
                throw new ArgumentNullException("testGenericService is null");
            }

            _testService = testGenericService;
             * */
        }

        //GET api/<controller>
        /// <summary>
        /// api/processes/{ProcessID}/scenarios/{scenarioID}/compute
        /// Diagnostic function to compute LCIA results for ALL methods for a given process.
        /// Should get removed; kept in place to inform LCIA Computation refactoring for #111
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="scenarioId"></param>
        /// <returns>FragmentLCIAModel (list) (broken!)</returns>
        [Route("api/processes/{ProcessID}/scenarios/{scenarioID}/compute")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<LCIAResult> LCIACompute(int processId, int scenarioId)
        {
            return _lciaComputationV2.LCIACompute(processId, scenarioId);
        }

        /// <summary>
        /// api/processes/{ProcessID}/scenarios/{scenarioID}/inventory
        /// Diagnostic function to return inventory results for a given process under a given scenario.
        /// Useful for developing + debugging dissipation modeling (#31)
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="scenarioId"></param>
        /// <returns>InventoryModel (list)</returns>
        [Route("api/processes/{ProcessID}/scenarios/{scenarioID}/inventory")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<InventoryModel> LCICompute(int processId, int scenarioId)
        {
            return _lciaComputationV2.ComputeProcessLCI(processId, scenarioId);
        }

        /// <summary>
        /// Diagnostic function to compute score cache entries for a given fragment and scenario
        /// but not write them to the cache.
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        [Route("api/fragments/{FragmentID}/scenarios/{scenarioID}/lcia")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<ScoreCache> LCIAFragmentCompute(int fragmentId, int scenarioId)
        {
            var sw = Stopwatch.StartNew();
            var scores = _fragmentLCIAComputation.FragmentLCIAComputeNoSave(fragmentId, scenarioId);
            sw.Stop();
            return scores;
        }

        /// <summary>
        /// Diagnostic function to compute a fragment and write both traversal and LCIA results to cache
        /// as needed.  This should be used for testing only; user-generated scenarios should be computed 
        /// via the UpdateScenario mechanism in ResourceServiceFacade; base scenarios should be populated
        /// at initialization (TODO) and never deleted.
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        [Route("api/fragments/{FragmentID}/scenarios/{scenarioID}/compute")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public void LCIAFragmentComputeSave(int fragmentId, int scenarioId)
        {
            var sw = Stopwatch.StartNew();
            _fragmentLCIAComputation.FragmentLCIAComputeSave(fragmentId, scenarioId);
            sw.Stop();
            return;// scores;
        }


        /*
        // Diagnostic functions
        /// <summary>
        /// api/fragmentflows/{fragmentFlowID}/scenarios/{scenarioID}/node
        /// Diagnostic function - terminates the requested fragmentflow under the given scenario.
        /// </summary>
        /// <param name="fragmentFlowID"></param>
        /// <param name="scenarioID"></param>
        /// <returns>FragmentNodeResource</returns>
        [Route("api/fragmentflows/{fragmentFlowID}/scenarios/{scenarioID}/node")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public FragmentNodeResource FindTerminus(int fragmentFlowID, int scenarioID)
        {
            return _testService.FindTerminus(fragmentFlowID, scenarioID);
        }

        /// <summary>
        /// Returns dependencies that complement a given FragmentFlow.
        /// FragmentFlow terminates in a child node, which can be a process or subfragment.  
        /// Returns all intermediate flows belonging to the child node except for the named inflow.
        /// </summary>
        /// <param name="fragmentFlowID"></param>
        /// <param name="scenarioID"></param>
        /// <returns>InventoryModel (List)</returns>
        [Route("api/fragmentflows/{fragmentFlowID}/scenarios/{scenarioID}/outflows")]
        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public IEnumerable<InventoryModel> GetDependencies(int fragmentFlowID, int scenarioID)
        {
            return _testService.GetDependencies(fragmentFlowID, scenarioID);
        }
        */


       
    }
}
