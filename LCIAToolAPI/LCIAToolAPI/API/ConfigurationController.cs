
using Entities.Models;
using LcaDataModel;
using Ninject;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using CalRecycleLCA.Services;

namespace LCAToolAPI.API
{
    /// <summary>
    /// Internal API routes for testing LCIA computation and ScoreCache control.  Creation [and deletion...] of
    /// Scenario Groups also happens here.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ConfigurationController : ApiController
    {
        [Inject]
        private readonly ILCIAComputationV2 _lciaComputationV2;
        [Inject]
        private readonly IFragmentLCIAComputation _fragmentLCIAComputation;
        [Inject]
        private readonly ICacheManager _CacheManager;
        //[Inject]
        //private readonly ITestGenericService _testService;

        /// <summary>
        /// Constructor for configuration + diagnostic controller.  Creates computation objects 
        /// via dependency injection.
        /// </summary>
        /// <param name="lciaComputationV2"></param>
        /// <param name="fragmentLCIAComputation"></param>
        /// <param name="cacheManager"></param>
        public ConfigurationController(
            ILCIAComputationV2 lciaComputationV2, 
            IFragmentLCIAComputation fragmentLCIAComputation,
            ICacheManager cacheManager)
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

            if (cacheManager == null)
            {
                throw new ArgumentNullException("cacheManager is null");
            }

            _CacheManager = cacheManager;

            /*
            if (testGenericService == null)
            {
                throw new ArgumentNullException("testGenericService is null");
            }

            _testService = testGenericService;
             * */
        }

        /// <summary>
        /// Runs through base case and existing scenarios, populating cache.
        /// Base case has cache populated for all fragments, all LCIA methods
        /// Scenarios have cache populated for top level fragment and descendents, all LCIA methods.
        /// </summary>
        [Route("config/init")]
        [HttpGet]
        public HttpResponseMessage InitializeCache()
        {
            var result = _CacheManager.InitializeCache();
            return Request.CreateResponse(HttpStatusCode.OK, String.Format("Computed {0} fragments, {1} scenarios.", result[0], result[1]));
        }

        /// <summary>
        /// Creates a new scenario group with a known "secret." Requests must use this secret as auth token when requesting scenarios belonging to this group.
        /// 
        /// Note that this should obviously use some more secure form of secret sharing.
        /// Note too that updating scenario groups (secret, visibility) is TODO.
        /// </summary>
        /// <param name="postdata"></param>
        /// <returns>ScenarioGroup resource with secret omitted.</returns>
        [Route("config/scenariogroups/add")]
        [AcceptVerbs("POST")]
        [HttpPost]
        public HttpResponseMessage CreateScenarioGroup([FromBody] ScenarioGroupResource postdata)
        {
            if (postdata == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No content provided.");
            if (String.IsNullOrEmpty(postdata.Secret))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No Secret provided.");
            if (String.IsNullOrEmpty(postdata.Name))
                postdata.Name = "New Scenario Group";
            return Request.CreateResponse(HttpStatusCode.OK,
                _CacheManager.CreateScenarioGroup(postdata));
        }

        /**
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
            return _lciaComputationV2.ComputeProcessEmissions(processId, scenarioId);
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
         * */

        /// <summary>
        /// Diagnostic function to compute a fragment and write both traversal and LCIA results to cache
        /// as needed.  This is useful for un-sticking a scenario that was marked "stale" but did not compute
        /// (e.g. due to an exception) and is returning 410 Conflict.  Normal operation is for user-generated 
        /// scenarios should be computed via the UpdateScenario mechanism in ResourceServiceFacade; base 
        /// scenarios should be populated via /config/init and never deleted.
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        [Route("config/fragments/{FragmentID}/scenarios/{scenarioID}/compute")]
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

        /// <summary>
        /// Clear NodeCache data by ScenarioID, and re-compute via ImplementScenarioChanges().
        /// </summary>
        /// <param name="scenarioId"></param>
        [Route("config/scenarios/{scenarioID:int}/clearnodecaches")]
        [HttpPost]
        public void ClearNodeCacheByScenario(int scenarioId)
        {
            _CacheManager.ClearNodeCacheByScenario(scenarioId);
        }

        /*
        /// <summary>
        /// Clear NodeCache data by ScenarioID and FragmentID
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fragmentId"></param>
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/clearnodecaches")]
        [HttpPost]
        public void ClearNodeCacheByScenarioAndFragment(int scenarioId, int fragmentId)
        {
            _ResourceService.ClearNodeCacheByScenarioAndFragment(scenarioId, fragmentId);
        }
        */

        /// <summary>
        /// Clear ScoreCache data by ScenarioID, and re-compute via ImplementScenarioChanges().
        /// </summary>
        /// <param name="scenarioId"></param>
        [Route("config/scenarios/{scenarioID:int}/clearscorecaches")]
        [HttpPost]
        public void ClearScoreCacheByScenario(int scenarioId)
        {
            _CacheManager.ClearScoreCacheByScenario(scenarioId);
        }

        /*
        /// <summary>
        /// Clear ScoreCache data by ScenarioID and FragmentID
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fragmentId"></param>
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/clearscorecaches")]
        [HttpPost]
        public void ClearScoreCacheByScenarioAndFragment(int scenarioId, int fragmentId)
        {
            _ResourceService.ClearScoreCacheByScenarioAndFragment(scenarioId, fragmentId);
        }
         * */


        /// <summary>
        /// Clear ScoreCache data by ScenarioID and LCIAMethodID, and re-compute via ImplementScenarioChanges().
        /// 
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="lciaMethodId"></param>
        [Route("config/scenarios/{scenarioID:int}/lciamethods/{lciaMethodID:int}/clearscorecaches")]
        [HttpPost]
        public void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId, int lciaMethodId)
        {
            _CacheManager.ClearScoreCacheByScenarioAndLCIAMethod(scenarioId, lciaMethodId);
        }


       
    }
}
