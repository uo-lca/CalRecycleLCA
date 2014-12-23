using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ninject;
using Entities.Models;
using CalRecycleLCA.Services;
using LcaDataModel;

namespace LCAToolAPI.API
{
    /// <summary>
    /// Controller for web API resources
    /// All routes used by the front-end visualizations will be defined here.
    /// </summary>
    public class ResourceController : ApiController
    {
        // mockup JSON that would be sent from Sabina's AngularJs app.
        private string addScenarioJSON = "{\"scenarios\":[" +
             "{\"scenarioID\":\"3\",\"scenarioGroupID\":\"1\",\"topLevelFragmentID\":\"8\",\"activityLevel\":\"8300\", \"name\":\"Test Scenario\", \"flowID\":\"373\", \"directionID\":\"2\"}" +
             "]}";

        private string updateScenarioJSON = "{\"scenarios\":[" +
           "{\"scenarioID\":\"3\",\"scenarioGroupID\":\"1\",\"topLevelFragmentID\":\"8\",\"activityLevel\":\"8500\", \"name\":\"Test Scenario\", \"flowID\":\"373\", \"directionID\":\"2\"}" +
           "]}";

        private string deleteScenarioJSON = "{\"scenarios\":[" +
         "{\"scenarioID\":\"3\"}" +
         "]}";

        private string deleteParamJSON = "{\"params\":[" +
         "{\"paramID\":\"3\"}" +
         "]}";

        private string addParamJSON = "{\"params\":[" +
             "{\"paramTypeID\":\"9\",\"scenarioID\":\"1\", \"name\":\"Test Param\", \"value\":\"6.99\", \"fragmentFlowID\":\"243\", \"flowID\":\"31\", \"flowPropertyID\":\"23\", \"processID\":\"373\", \"lciaMethodID\":\"373\", \"conservation\":\"true\", \"dependencyParamID\":\"20\",}" +
             "]}";

        private string updateParamJSON = "{\"params\":[" +
          "{\"paramID\":\"10\", \"paramTypeID\":\"1\",\"scenarioID\":\"1\", \"name\":\"Test Param\", \"value\":\"6.99\", \"fragmentFlowID\":\"243\", \"flowID\":\"373\", \"flowPropertyID\":\"373\", \"processID\":\"373\", \"lciaMethodID\":\"373\", \"conservation\":\"true\", \"dependencyParamID\":\"20\",}" +
          "]}";

        [Inject]
        private readonly IResourceServiceFacade _ResourceService;

        public ResourceController(ResourceServiceFacade resourceService)
        {
            if (resourceService == null)
            {
                throw new ArgumentNullException("resourceService");
            }
            _ResourceService = resourceService;
        }

        /// <summary>
        /// List all flows in the database.
        /// </summary>
        /// <returns></returns>
        [Route("api/flows")]
        [Route("api/flowtypes/{flowtypeID:int}/flows")]
        [HttpGet]
        public IEnumerable<FlowResource> GetFlows(int flowtypeID = 0)
        {
            return _ResourceService.GetFlows(flowtypeID);
        }

        /// <summary>
        /// List enumerated flow types.  1-- Intermediate.  2-- Elementary.
        /// </summary>
        /// <returns></returns>
        [Route("api/flowTypes")]
        [HttpGet]
        public IEnumerable<FlowTypeResource> GetFlowTypes() {
            return _ResourceService.GetFlowTypes();
        }

        /// <summary>
        /// List all flowproperties in the database.
        /// </summary>
        /// <returns></returns>
        [Route("api/flowproperties")]
        [HttpGet]
        public IEnumerable<FlowPropertyResource> GetFlowProperties()
        {
            return _ResourceService.GetFlowProperties();
        }

        /// <summary>
        /// Get the list of all scenarios eligible to be viewed given the connection's authorization.
        /// Note: authorization is presently not implemented.
        /// </summary>
        /// <returns></returns>
        [Route("api/scenarios")]
        [HttpGet]
        public IEnumerable<ScenarioResource> GetScenarios()
        {
            // need auth here to determine remote user's scenario group.  
            // if unprivileged:
            //return _ResourceService.GetScenarios(userGroupID);
            // else:
            return _ResourceService.GetScenarios();
        }

        /// <summary>
        /// Get the list of all fragments in the DB.
        /// </summary>
        /// <returns></returns>
        [Route("api/fragments")]
        [HttpGet]
        public IEnumerable<FragmentResource> GetFragments() {
            return _ResourceService.GetFragmentResources();
        }

        /// <summary>
        /// a little-seen stub method to return a single FragmentResource.
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <returns></returns>
        [Route("api/fragments/{fragmentID:int}")]
        [HttpGet]
        public FragmentResource GetFragment(int fragmentID) {
            FragmentResource fr = _ResourceService.GetFragmentResource(fragmentID);
            if (fr == null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return fr;
        }

        // // Scenarios //////////////////////////////////////////////////////////////
        // [Route("api/scenarios")]
        // [HttpGet]
        // public IEnumerable<ScenarioResource> GetScenarios(int userID) {
        //     // need to get userID or scenarioGroupID from server auth
        //     return _ResourceService.GetScenarios(userID);
        // }

        // [Route("api/scenarios/{scenarioID:int}/params")]
        // [HttpGet]
        // public IEnumerable<ScenarioResource> GetScenarioParams(int scenarioID) {
        //     // need to get userID or scenarioGroupID from server auth
        //     return _ResourceService.GetScenarioParams(scenarioID);
        // }

        // [Route("api/scenarios/{scenarioID:int}/substitutions")]
        // [HttpGet]
        // public IEnumerable<ScenarioSubstitutionResource> GetScenarioSubs(int scenarioID) {
        //     // need to get userID or scenarioGroupID from server auth
        //     return _ResourceService.GetScenarioSubs(scenarioID);
        // }

        // Fragments //////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns a list of FragmentFlows belonging to a fragment.. i.e. the links in the 
        /// fragment tree structure.  Optionally specify a scenarioID.
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="scenarioID"></param>
        /// <returns>FragmentFlowResource array</returns>
        [Route("api/fragments/{fragmentID:int}/fragmentflows")]
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/fragmentflows")]
        [HttpGet]
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = Scenario.MODEL_BASE_CASE_ID) {
            // Use default scenario
            return _ResourceService.GetFragmentFlowResources(fragmentID, scenarioID); 
        }
        /*
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/fragmentflows")]
        [HttpGet]
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID) {
            return _ResourceService.GetFragmentFlowResources(fragmentID, scenarioID);
        }*/

        // lists indefinite flows associated with a fragment
        [Route("api/fragments/{fragmentID:int}/flows")]
        [HttpGet]
        public IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID) {
            return _ResourceService.GetFlowsByFragment(fragmentID);
        }

        /// <summary>
        /// Return a table of FragmentStage resources.  FragmentStages are grouping units for nodes
        /// in a fragment for the purposes of LCIA reporting.  Each fragment has its own distinct
        /// stages, and every impact-generating flow belongs to one stage.
        /// </summary>
        /// <param name="fragmentID">if present, report only stages belonging to the specified fragment</param>
        /// <returns>FragmentStageResource array</returns>
        [Route("api/fragmentstages")]
        [Route("api/stages")]
        [Route("api/fragments/{fragmentID:int}/fragmentstages")]
        [Route("api/fragments/{fragmentID:int}/stages")]
        [HttpGet]
        public IEnumerable<FragmentStageResource> GetStagesByFragment(int fragmentID = 0)
        {
            return _ResourceService.GetStagesByFragment(fragmentID);
        }
        
        // lists all flow properties associated with fragment flows
        [Route("api/fragments/{fragmentID:int}/flowproperties")]
        [HttpGet]
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID) {
            return _ResourceService.GetFlowPropertiesByFragment(fragmentID);
        }

        // reports lcia results for all scenarios in a scenario group 
        // TODO: Add scenario group filter
        [Route("api/fragments/{fragmentID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID) {
             return _ResourceService.GetFragmentLCIAResultsAllScenarios(fragmentID, lciaMethodID);
        }

        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID) {
            return _ResourceService.GetFragmentLCIAResults(fragmentID, lciaMethodID, scenarioID);
        }

        // LCIA Metadata ////////////////////////////////////////////////////////////
        [Route("api/impactcategories")]
        [HttpGet]
        public IEnumerable<ImpactCategoryResource> ImpactCategories() {
            return _ResourceService.GetImpactCategories();
        }

        [Route("api/impactcategories/{impactCategoryID:int}/lciamethods")]
        [HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodsByImpactCategory(int impactCategoryID) {
            return _ResourceService.GetActiveLCIAMethodResources(impactCategoryID);
        }

        [Route("api/lciamethods")]
        [HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodResources() {
            return _ResourceService.GetActiveLCIAMethodResources();
        }

        [Route("api/lciamethods/{lciaMethodId:int}/factors")]
        [Route("api/lciamethods/{lciaMethodId:int}/lciafactors")]
        [HttpGet]
        public IEnumerable<LCIAFactorResource> GetLCIAFactors(int lciaMethodId)
        {
            return _ResourceService.GetLCIAFactors(lciaMethodId);
        }

        // Process metadata /////////////////////////////////////////////////////////
        [Route("api/processes")]
        [HttpGet]
        public IEnumerable<ProcessResource> GetProcesses() {
            return _ResourceService.GetProcesses();
        }

        // list processes having any of the specified flowtypeID 
        // 1=elementary, for identifying flows with emissions
        [Route("api/flowtypes/{flowTypeID:int}/processes")]
        [HttpGet]
        public IEnumerable<ProcessResource> GetProcesses(int flowTypeID) {
            return _ResourceService.GetProcesses(flowTypeID);
        }

        // list definite (i.e. quantity-bearing) flows associated with a process
        // access control needed here
        [Route("api/processes/{processID:int}/processflows")]
        [HttpGet]
        public IEnumerable<ProcessFlowResource> GetProcessFlows(int processID) {
            return _ResourceService.GetProcessFlows(processID);
        }

        // ???
        [Route("api/processes/{processID:int}/flowproperties")]
        [HttpGet]
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID) {
            return _ResourceService.GetFlowPropertiesByProcess(processID);
        }

        // report LCIA results as a table 
        // access control needed here
        [Route("api/processes/{processID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID)
        {
            return _ResourceService.GetProcessLCIAResult(processID, lciaMethodID);
        }

        // as above w/ scenario
        [Route("api/scenarios/{scenarioID:int}/processes/{processID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID, int scenarioID)
        {
            return _ResourceService.GetProcessLCIAResult(processID, lciaMethodID, scenarioID);
        }

        /// <summary>
        /// Clear NodeCache data by ScenarioID
        /// </summary>
        /// <param name="scenarioId"></param>
        [Route("api/scenarios/{scenarioID:int}/clearnodecaches")]
        [HttpPost]
        public void ClearNodeCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            _ResourceService.ClearNodeCacheByScenario(scenarioId);
        }

        /// <summary>
        /// Clear NodeCache data by ScenarioID and FragmentID
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fragmentId"></param>
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/clearnodecaches")]
        [HttpPost]
        public void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _ResourceService.ClearNodeCacheByScenarioAndFragment(scenarioId, fragmentId);
        }

        /// <summary>
        /// Clear ScoreCache data by ScenarioID
        /// </summary>
        /// <param name="scenarioId"></param>
        [Route("api/scenarios/{scenarioID:int}/clearscorecaches")]
        [HttpPost]
        public void ClearScoreCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            _ResourceService.ClearScoreCacheByScenario(scenarioId);
        }

        /// <summary>
        /// Clear ScoreCache data by ScenarioID and FragmentID
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fragmentId"></param>
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/clearscorecaches")]
        [HttpPost]
        public void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _ResourceService.ClearScoreCacheByScenarioAndFragment(scenarioId, fragmentId);
        }

        /// <summary>
        /// Clear ScoreCache data by ScenarioID and LCIAMethodID
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fragmentId"></param>
        [Route("api/scenarios/{scenarioID:int}/lciamethods/{lciaMethodID:int}/clearscorecaches")]
        [HttpPost]
        public void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int lciaMethodId = 0)
        {
            _ResourceService.ClearScoreCacheByScenarioAndLCIAMethod(scenarioId, lciaMethodId);
        }

    
        //[Authorize]
        [Route("api/scenarios/{scenarioId}/params")]
        [AcceptVerbs("GET")]
        public IEnumerable<ParamResource> GetScenarioParams(int scenarioId)
        {
            return _ResourceService.GetParams(scenarioId);
        }

        //[Authorize]
        [Route("api/scenarios/{scenarioId}/params/{paramId}")]
        [AcceptVerbs("GET")]
        public IEnumerable<ParamResource> GetScenarioParam(int scenarioId, int paramId)
        {
            // leakproof linq
            return _ResourceService.GetParams(scenarioId).Where(k => k.ParamID == paramId);
        }

        //[Authorize]
        [Route("api/scenarios")]
        [AcceptVerbs("POST")]
        [HttpPost]
        public void AddScenario(int scenarioId)
        {
            // need to authorize this
            _ResourceService.AddScenario(addScenarioJSON);
        }

        [Authorize]
        [Route("api/scenarios/{scenarioId}")]
        [AcceptVerbs("PUT")]
        [HttpPut]
        public void UpdateScenario(int scenarioId)
        {
            _ResourceService.UpdateScenario(updateScenarioJSON);
        }

        [Route("api/deleteScenario")]
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public void DeleteScenario()
        {
            _ResourceService.DeleteScenario(deleteScenarioJSON);
        }

        [Route("api/deleteParam")]
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public void DeleteParam()
        {
            _ResourceService.DeleteParam(deleteParamJSON);
        }

        [Route("api/addParam")]
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public void AddParam()
        {
            _ResourceService.AddParam(addParamJSON);
        }

        [Route("api/updateParam")]
        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public void UpdateParam()
        {
            _ResourceService.UpdateParam(updateParamJSON);
        }

        /* TODO:
         * 
         * Add route to get Process LCIA results for all methods
         * [Route("api/processes/{processID:int}")] // to report what?
         X [Route("api/fragments/{fragmentID:int}/lciaresults")]    
         X [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/lciaresults")]    
         X [Route("api/scenariogroups/{scenarioID:int}/scenarios")]    
         * 
         */
    }
}
