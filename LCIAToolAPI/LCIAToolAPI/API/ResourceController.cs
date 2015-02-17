using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Ninject;
using Entities.Models;
using CalRecycleLCA.Services;
using LcaDataModel;

namespace LCAToolAPI.API
{
    /// <summary>
    /// Controller for web API resources
    /// All routes used by the front-end visualizations will be defined here.
    /// Enabling CORS to accept all comers-- control access at the IP or host layer.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ResourceController : ApiController
    {
        [Inject]
        private readonly IResourceServiceFacade _ResourceService;

        [Inject]
        private readonly IScenarioGroupService _ScenarioGroupService;

        [Inject]
        private readonly IDocuService _DocuService;

        [Inject]
        private readonly IParamService _ParamService;

        private String ToolVersion()
        {
            string gitVersion = String.Empty;
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("LCAToolAPI." + "version.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                gitVersion = reader.ReadToEnd();
            }
            return gitVersion.TrimEnd('\r' , '\n');

        }

        private IEnumerable<LCIAMethodResource> Decorate(IEnumerable<LCIAMethodResource> resource)
        {
            foreach (var k in resource)
                k.Links.AddRange(_DocuService.ResourceLinks(ActionContext, k));
            return resource;
        }
        private IEnumerable<ProcessResource> Decorate(IEnumerable<ProcessResource> resource)
        {
            foreach (var k in resource)
                k.Links.AddRange(_DocuService.ResourceLinks(ActionContext, k));
            return resource;
        }
        private IEnumerable<FlowResource> Decorate(IEnumerable<FlowResource> resource)
        {
            foreach (var k in resource)
                k.Links.AddRange(_DocuService.ResourceLinks(ActionContext, k));
            return resource;
        }
        private IEnumerable<FlowPropertyResource> Decorate(IEnumerable<FlowPropertyResource> resource)
        {
            foreach (var k in resource)
                k.Links.AddRange(_DocuService.ResourceLinks(ActionContext, k));
            return resource;
        }

        /// <summary>
        /// Dependency injection
        /// </summary>
        /// <param name="resourceService">provides all services except authorization</param>
        /// <param name="scenarioGroupService">provides authorization</param>
        /// <param name="docuService">provides documentary information</param>
        /// <param name="paramService">provides param data</param>
        public ResourceController(IResourceServiceFacade resourceService,
            IScenarioGroupService scenarioGroupService,
            IDocuService docuService,
            IParamService paramService)
        {
            if (resourceService == null)
            {
                throw new ArgumentNullException("resourceService");
            }
            _ResourceService = resourceService;
            _ScenarioGroupService = scenarioGroupService;
            _DocuService = docuService;
            _ParamService = paramService;
        }

        /// <summary>
        /// In lieu of HATEOAS, here's a link to documentation. 
        /// </summary>
        /// <returns>Link to documentation.</returns>
        [Route("api")]
        [Route("api/api")]
        [Route("api/help")]
        [HttpGet]
        public ApiInfo GetHelp()
        {
            var apiInfo = _DocuService.ApiInfo(RequestContext);
            apiInfo.Version = ToolVersion();
            return apiInfo;
        }

        /// <summary>
        /// Returns a plain string containing the API version from git describe.
        /// </summary>
        /// <returns></returns>
        [Route("api/version")]
        [HttpGet]
        public String GetVersion()
        {
            return ToolVersion();
        }

        /// <summary>
        /// List all flows in the database.  
        /// </summary>
        /// <returns></returns>
        [Route("api/flows")]
        [Route("api/flowtypes/{flowtypeID:int}/flows")]
        [HttpGet]
        public IEnumerable<FlowResource> GetFlows(int flowTypeId = 0)
        {
            return Decorate(_ResourceService.GetFlows(flowTypeId));
        }

        /// <summary>
        /// flow by ID
        /// </summary>
        /// <param name="flowId">int</param>
        /// <returns>list of FlowResource</returns>
        [Route("api/flows/{flowId}")]
        [HttpGet]
        public IEnumerable<FlowResource> GetFlow(int flowId)
        {
            return Decorate(_ResourceService.GetFlow(flowId));
            /*
            var flows = _ResourceService.GetFlow(flowId);
            foreach (FlowResource k in flows)
            {
                var q = _DocuService.ResourceLinks(ActionContext, k);
                k.Links.AddRange(q);
            }
            return flows;
             * */
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
            return Decorate(_ResourceService.GetFlowProperties());
        }

        /// <summary>
        /// one flow property
        /// </summary>
        /// <param name="fpid"></param>
        /// <returns></returns>
        [Route("api/flowproperties/{fpid}")]
        [HttpGet]
        public IEnumerable<FlowPropertyResource> GetFlowProperties(int fpid)
        {
            return Decorate(_ResourceService.GetFlowProperties().Where(k => k.FlowPropertyID == fpid).ToList());
        }

        /// <summary>
        /// List specific flowproperties for a flow
        /// </summary>
        /// <returns>FlowPropertyMagnitude list</returns>
        [Route("api/flows/{flowId}/flowproperties")]
        [HttpGet]
        public IEnumerable<FlowPropertyMagnitude> GetFlowFlowProperties(int flowId)
        {
            return _ResourceService.GetFlowFlowProperties(flowId);
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
        /// Get a single fragment.
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
        /// fragment tree structure.  .
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <returns>FragmentFlowResource array</returns>
        [Route("api/fragments/{fragmentID:int}/fragmentflows")]
        [HttpGet]
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID) {
            // Use default scenario
            return _ResourceService.GetFragmentFlowResources(fragmentID); 
        }

        /// <summary>
        /// scenario-specific.
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="scenarioID"></param>
        /// <returns></returns>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/fragmentflows")]
        [HttpGet]
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = Scenario.MODEL_BASE_CASE_ID)
        {
            if (_ScenarioGroupService.CanGet(RequestContext))
                return _ResourceService.GetFragmentFlowResources(fragmentID, scenarioID);
            else
                return null;
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
        [CalRecycleAuthorize]
        [Route("api/fragments/{fragmentID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID) {
            int authGroup = (int)_ScenarioGroupService.CheckAuthorizedGroup(RequestContext);
            if (authGroup == 0)
                return _ResourceService.GetFragmentLCIAResultsAllScenarios(fragmentID, lciaMethodID);
            else
                return _ResourceService.GetFragmentLCIAResultsAllScenarios(fragmentID, lciaMethodID, authGroup);
        }

        /// <summary>
        /// Fragment LCIA results across all methods under base scenario
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <returns>LCIAResultResource list</returns>
        [Route("api/fragments/{fragmentID:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllMethods(int fragmentID)
        {
            return _ResourceService.GetFragmentLCIAResultsAllMethods(fragmentID);
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
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID) {
            if (_ScenarioGroupService.CanGet(RequestContext))
                return _ResourceService.GetFragmentLCIAResults(fragmentID, lciaMethodID, scenarioID);
            else
                return null;
        }

        /// <summary>
        /// Fragment LCIA results, all methods, for a given scenario
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="scenarioID"></param>
        /// <returns>LCIAResultResource list</returns>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllMethods(int fragmentID, int scenarioID)
        {
            if (_ScenarioGroupService.CanGet(RequestContext))
                return _ResourceService.GetFragmentLCIAResultsAllMethods(fragmentID, scenarioID);
            else
                return null;
        }

        /// <summary>
        /// Given a Param ID, returns sensitivity results-- dr/dx for lcia score r
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="paramId"></param>
        /// <returns></returns>
        [CalRecycleAuthorize]
        [Route("api/fragments/{fragmentId:int}/params/{paramId:int}/sensitivity")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetFragmentSensitivity(int fragmentId, int paramId)
        {
            int scenarioId = _ParamService.Queryable().Where(p => p.ParamID == paramId)
                .Select(p => p.ScenarioID).First();
            if (_ScenarioGroupService.CanGet(RequestContext, scenarioId))
                return _ResourceService.GetFragmentSensitivity(fragmentId, paramId);
            else
                return null;
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
            return Decorate(_ResourceService.GetActiveLCIAMethodResources(impactCategoryID));
        }

        /// <summary>
        /// GET api/lciamethods
        /// List all LCIA methods.
        /// </summary>
        /// <returns>LCIAMethodResource (list)</returns>
        [Route("api/lciamethods")]
        [HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodResources() {
            return Decorate(_ResourceService.GetActiveLCIAMethodResources());
        }

        /// <summary>
        /// GET api/lciamethods
        /// List all LCIA methods.
        /// </summary>
        /// <param name="lciaMethodId"></param>
        /// <returns></returns>
        [Route("api/lciamethods/{lciaMethodId}")]
        [HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodResource(int lciaMethodId)
        {
            return Decorate(_ResourceService.GetActiveLCIAMethodResources()
                .Where(k => k.LCIAMethodID == lciaMethodId).ToList());
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
        /// <summary>
        /// List of processes
        /// </summary>
        /// <returns>list of ProcessResource</returns>
        [Route("api/processes")]
        [HttpGet]
        public IEnumerable<ProcessResource> GetProcesses() {
            return Decorate(_ResourceService.GetProcesses());
        }

        /// <summary>
        /// an individual process
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        [Route("api/processes/{processId}")]
        [HttpGet]
        public IEnumerable<ProcessResource> GetProcess(int processId)
        {
            return Decorate(_ResourceService.GetProcess(processId));
        }

        // 
        // 
        /// <summary>
        /// list processes having any of the specified flowtypeID 
        /// 1=intermediate should return all processes
        /// 2 will return only processes with emissions
        /// </summary>
        /// <param name="flowTypeID">1=intermediate; 2=elementary</param>
        /// <returns></returns>
        [Route("api/flowtypes/{flowTypeID:int}/processes")]
        [HttpGet]
        public IEnumerable<ProcessResource> GetProcesses(int flowTypeID) {
            return Decorate(_ResourceService.GetProcesses(flowTypeID));
        }

        
        /// <summary>
        /// list definite (i.e. quantity-bearing) flows associated with a process
        /// (privacy protected in ProcessService)
        /// </summary>
        /// <param name="processID"></param>
        /// <returns>ProcessFlowResource list</returns>
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

        /// <summary>
        /// LCIA results for all methods
        /// </summary>
        /// <param name="processID"></param>
        /// <returns>LCIAResultResource list</returns>
        [Route("api/processes/{processID:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetProcessLCIAResults(int processID)
        {
            return _ResourceService.GetProcessLCIAResults(processID);
        }

        // report LCIA results as a table 
        // access control needed here
        /// <summary>
        /// Report detailed process LCIA results for a specific method. (privacy protected in service layer)
        /// </summary>
        /// <param name="processID">the process ID</param>
        /// <param name="lciaMethodID">the LCIAMethod ID</param>
        /// <returns>ProcessFlowResource list</returns>
        [Route("api/processes/{processID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID)
        {
            return _ResourceService.GetProcessLCIAResult(processID, lciaMethodID);
        }


        /// <summary>
        /// As above with scenario. 
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="scenarioId"></param>
        /// <returns>LCIAResultResource list</returns>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioId}/processes/{processId:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetProcessLCIAResults(int processId, int scenarioId)
        {
            return _ResourceService.GetProcessLCIAResults(processId, scenarioId);
        }

        /// <summary>
        /// As above with scenario. 
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="lciaMethodID"></param>
        /// <param name="scenarioID"></param>
        /// <returns></returns>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioID:int}/processes/{processID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID, int scenarioID)
        {
            if (_ScenarioGroupService.CanGet(RequestContext))
                return _ResourceService.GetProcessLCIAResult(processID, lciaMethodID, scenarioID);
            else
                return null;
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
        /// <param name="putdata"></param>
        /// <returns></returns>
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
        public ParamResource AddParam(int scenarioId, [FromBody] ParamResource postParam)
        {
            if (_ScenarioGroupService.CanAlter(RequestContext)) 
               return _ResourceService.AddParam(scenarioId, postParam).FirstOrDefault();
            
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Update entire param collection for a scenario.  Required when more than one concurrent 
        /// update is submitted.  Client error to issue concurrent PUT or POST requests for the same scenario. 
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioId}/params")]
        [AcceptVerbs("PUT")]
        [HttpPut]
        public IEnumerable<ParamResource> UpdateParams(int scenarioId, [FromBody] IEnumerable<ParamResource> putParams)
        {
            if (_ScenarioGroupService.CanAlter(RequestContext))
                return _ResourceService.UpdateParams(scenarioId, putParams);
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }



        /// <summary>
        /// PUT api/scenarios/{scenarioId}/params/{paramId}
        /// Update param.  only name or value fields updated.
        /// </summary>
        [CalRecycleAuthorize]
        [Route("api/scenarios/{scenarioId}/params/{paramId}")]
        [AcceptVerbs("PUT")]
        [HttpPost]
        public ParamResource UpdateParam(int scenarioId, int paramId, [FromBody] ParamResource putParam)
        {
            if (_ScenarioGroupService.CanAlter(RequestContext))
                return _ResourceService.UpdateParam(scenarioId, paramId, putParam).FirstOrDefault();
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Delete a param.  must be authorized for the scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="paramId"></param>
        /// <param name="putParam"></param>
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
