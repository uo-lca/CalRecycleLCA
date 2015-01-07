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

        [Inject]
        private readonly IScenarioGroupService _ScenarioGroupService;

        public ResourceController(IResourceServiceFacade resourceService,
            IScenarioGroupService scenarioGroupService)
        {
            if (resourceService == null)
            {
                throw new ArgumentNullException("resourceService");
            }
            _ResourceService = resourceService;
            _ScenarioGroupService = scenarioGroupService;
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

        [Route("api/flows/{flowId}")]
        [HttpGet]
        public IEnumerable<FlowResource> GetFlow(int flowId)
        {
            return _ResourceService.GetFlow(flowId);
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

        /// <summary>
        /// GET api/fragments/{fragmentID:int}/flows
        /// lists indefinite flows associated with a fragment
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <returns>FlowResource (list) with classification info appended</returns>
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
        
        
        /// <summary>
        /// GET api/fragments/{fragmentID:int}/flowproperties
        /// lists all flow properties associated with fragment flows in a certain fragment.
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <returns>FlowPropertyResource (list)</returns>
        [Route("api/fragments/{fragmentID:int}/flowproperties")]
        [HttpGet]
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID) {
            return _ResourceService.GetFlowPropertiesByFragment(fragmentID);
        }

        // reports lcia results for all scenarios in a scenario group 
        // TODO: Add scenario group filter
        /// <summary>
        /// GET api/fragments/{fragmentID:int}/lciamethods/{lciaMethodID:int}/lciaresults
        /// reports lcia results for all scenarios visible to the user. for a single LCIA method
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="lciaMethodID"></param>
        /// <returns>LCIAResultResource (list)</returns>
        [Route("api/fragments/{fragmentID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID) {
             return _ResourceService.GetFragmentLCIAResultsAllScenarios(fragmentID, lciaMethodID);
        }

        /// <summary>
        /// GET api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/lciamethods/{lciaMethodID:int}/lciaresults
        /// Compute LCIA results for a fragment and sub-fragments, under a particular scenario.
        /// requires authentication.  the ScenarioGroup for the Scenario specified must be 1 or authorizedScenarioGroup 
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="lciaMethodID"></param>
        /// <param name="scenarioID"></param>
        /// <returns>LCIAResultResource</returns>
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID) {
            return _ResourceService.GetFragmentLCIAResults(fragmentID, lciaMethodID, scenarioID);
        }

        // LCIA Metadata ////////////////////////////////////////////////////////////
        /// <summary>
        /// GET api/impactcategories
        /// List impact categories-- these were manually extracted from the ELCD-LCIA archive
        /// </summary>
        /// <returns>ImpactCategoryResource (list)</returns>
        [Route("api/impactcategories")]
        [HttpGet]
        public IEnumerable<ImpactCategoryResource> ImpactCategories() {
            return _ResourceService.GetImpactCategories();
        }

        /// <summary>
        /// GET api/impactcategories/{impactCategoryID:int}/lciamethods
        /// List all LCIA methods in a given category.
        /// </summary>
        /// <param name="impactCategoryID"></param>
        /// <returns>LCIAMethodResource (list)</returns>
        [Route("api/impactcategories/{impactCategoryID:int}/lciamethods")]
        [HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodsByImpactCategory(int impactCategoryID) {
            return _ResourceService.GetActiveLCIAMethodResources(impactCategoryID);
        }

        /// <summary>
        /// GET api/lciamethods
        /// List all LCIA methods.
        /// </summary>
        /// <returns>LCIAMethodResource (list)</returns>
        [Route("api/lciamethods")]
        [HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodResources() {
            return _ResourceService.GetActiveLCIAMethodResources();
        }

        /// <summary>
        /// GET api/lciamethods/{lciaMethodId:int}/factors
        /// GET api/lciamethods/{lciaMethodId:int}/lciafactors
        /// List LCIA factors for a given method. Only factors with matching flows, with null
        /// geographies, are returned.
        /// </summary>
        /// <param name="lciaMethodId"></param>
        /// <returns>LCIAFactorResource (list)</returns>
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
        /// <summary>
        /// GET api/processes/{processID:int}/flowproperties
        /// return a list of flow properties found in flows belonging to the process.
        /// </summary>
        /// <param name="processID"></param>
        /// <returns>FlowPropertyResource (list)</returns>
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
        public void ClearNodeCacheByScenario(int scenarioId)
        {
            _ResourceService.ClearNodeCacheByScenario(scenarioId);
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
        /// Clear ScoreCache data by ScenarioID
        /// </summary>
        /// <param name="scenarioId"></param>
        [Route("api/scenarios/{scenarioID:int}/clearscorecaches")]
        [HttpPost]
        public void ClearScoreCacheByScenario(int scenarioId)
        {
            _ResourceService.ClearScoreCacheByScenario(scenarioId);
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
        /// Clear ScoreCache data by ScenarioID and LCIAMethodID
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fragmentId"></param>
        [Route("api/scenarios/{scenarioID:int}/lciamethods/{lciaMethodID:int}/clearscorecaches")]
        [HttpPost]
        public void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId, int lciaMethodId)
        {
            _ResourceService.ClearScoreCacheByScenarioAndLCIAMethod(scenarioId, lciaMethodId);
        }

    
        //[Authorize]
        /// <summary>
        /// GET api/scenarios/{scenarioId}/params
        /// Returns a list of params belonging to the given scenario
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns>ParamResource (list)</returns>
        [Route("api/scenarios/{scenarioId}/params")]
        [AcceptVerbs("GET")]
        public IEnumerable<ParamResource> GetScenarioParams(int scenarioId)
        {
            return _ResourceService.GetParams(scenarioId);
        }

        
        //[Authorize]
        /// <summary>
        /// GET api/scenarios/{scenarioId}/params/{paramId}
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="paramId"></param>
        /// <returns>ParamResource</returns>
        [Route("api/scenarios/{scenarioId}/params/{paramId}")]
        [AcceptVerbs("GET")]
        public ParamResource GetScenarioParam(int scenarioId, int paramId)
        {
            // leakproof linq
            return _ResourceService.GetParams(scenarioId).Where(k => k.ParamID == paramId).FirstOrDefault();
        }

        /// <summary>
        /// GET api/scenariogroups 
        /// Get the scenario groups authorized by the connection.
        /// </summary>
        /// <returns></returns>
        [Route("api/scenariogroup")]
        [Route("api/scenariogroups")]
        [CalRecycleAuthorize]
        [HttpGet]
        public IEnumerable<ScenarioGroupResource> GetScenarioGroups()
        {
            return _ScenarioGroupService.AuthorizedGroups(RequestContext);
        }

        /// <summary>
        /// GET api/scenarios
        /// Get the list of all scenarios eligible to be viewed given the connection's authorization.
        /// Note: authorization is presently not implemented.
        /// </summary>
        /// <returns></returns>
        [Route("api/scenarios")]
        [CalRecycleAuthorize]
        [HttpGet]
        public IEnumerable<ScenarioResource> GetScenarios()
        {
            // need auth here to determine remote user's scenario group.  
            // if unprivileged:
            //return _ResourceService.GetScenarios(userGroupID);
            // else:
            return _ResourceService.GetScenarios(_ScenarioGroupService.CheckAuthorizedGroup(RequestContext));
        }

        /// <summary>
        /// GET api/scenarios
        /// Get the list of all scenarios eligible to be viewed given the connection's authorization.
        /// Note: authorization is presently not implemented.
        /// </summary>
        /// <returns></returns>
        [Route("api/scenarios/{scenarioId}")]
        [CalRecycleAuthorize]
        [HttpGet]
        public ScenarioResource GetScenario(int scenarioId)
        {

            // need auth here to determine remote user's scenario group.  
            // if unprivileged:
            //return _ResourceService.GetScenarios(userGroupID);
            // else:
            if (_ScenarioGroupService.CanGet(RequestContext))
                return _ResourceService.GetScenarios().Where(k => k.ScenarioID == scenarioId).FirstOrDefault();
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        
        //[Authorize]
        /// <summary>
        /// POST api/scenarios
        /// Creates a new scenario. not yet tested. Scenario group should be determined during authorization,
        /// and checked for consistency with post data.
        /// </summary>
        /// <returns>ScenarioResource for created scenario</returns>
        [CalRecycleAuthorize]
        [Route("api/scenarios")]
        [AcceptVerbs("POST")]
        [HttpPost]
        public ScenarioResource AddScenario([FromBody] ScenarioResource postdata)
        {
            int scenarioId;
            int? authGroup = _ScenarioGroupService.CheckAuthorizedGroup(RequestContext);
            // need to authorize this
            if (authGroup != 0 && authGroup != null)
            {
                var foo = Request.Content.ToString();
                scenarioId = _ResourceService.AddScenario(postdata, (int)authGroup);
                return _ResourceService.GetScenarios().Where(k => k.ScenarioID == scenarioId).FirstOrDefault(); 
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized); 

        }

        /// <summary>
        /// PUT api/scenarios/{scenarioId}
        /// Update a scenario. Requires authorization. Return the updated scenario
        /// </summary>
        /// <param name="scenarioId"></param>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioId}")]
        [AcceptVerbs("PUT")]
        [HttpPut]
        public ScenarioResource UpdateScenario(int scenarioId, [FromBody] ScenarioResource putdata)
        {
            if (_ScenarioGroupService.CanAlter(RequestContext))
            {
                if (_ResourceService.UpdateScenario(scenarioId, putdata))
                    return _ResourceService.GetScenarios().Where(k => k.ScenarioID == scenarioId).FirstOrDefault();
                else
                    return null;
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// DELETE api/scenarios/{scenarioId}
        /// </summary>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioId}")]
        [AcceptVerbs("DELETE")]
        public void DeleteScenario(int scenarioId)
        {
            if (_ScenarioGroupService.CanAlter(RequestContext))
                _ResourceService.DeleteScenario(scenarioId);
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized); 

        }

        /*
        /// <summary>
        /// DELETE api/scenarios/{scenarioId}/params/{paramId}
        /// Deletes the named parameter from the table
        /// not yet implemented
        /// </summary>
        [Authorize]
        [Route("api/scenarios/{scenarioId}/params/{paramId}")]
        [AcceptVerbs("DELETE")]
        [HttpPost]
        public void DeleteParam()
        {
            _ResourceService.DeleteParam(deleteParamJSON);
        }
         * */

        /// <summary>
        /// POST api/scenarios/{scenarioId}/params
        /// Create a new param belonging to the named Scenario. must be authenticated-- Scenario.ScenarioGroupID must 
        /// match the authorizedScenarioGroup;
        /// </summary>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioId}/params")]
        [AcceptVerbs("POST")]
        [HttpPost]
        public IEnumerable<ParamResource> AddParam(int scenarioId, [FromBody] ParamResource postParam)
        {
            if (_ScenarioGroupService.CanAlter(RequestContext))
                return _ResourceService.AddParam(scenarioId, postParam);
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// PUT api/scenarios/{scenarioId}/params/{paramId}
        /// Update param.  not yet implemented.
        /// </summary>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioId}/params/{paramId}")]
        [AcceptVerbs("PUT")]
        [HttpPost]
        public IEnumerable<ParamResource> UpdateParam(int scenarioId, int paramId, [FromBody] ParamResource putParam)
        {
            if (_ScenarioGroupService.CanAlter(RequestContext))
                return _ResourceService.UpdateParam(scenarioId, paramId, putParam);
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioId}/params/{paramId}")]
        [AcceptVerbs("DELETE")]
        [HttpPost]
        public void DeleteParam(int scenarioId, int paramId, [FromBody] ParamResource putParam)
        {
            if (_ScenarioGroupService.CanAlter(RequestContext))
                _ResourceService.DeleteParam(scenarioId, paramId);
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
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
