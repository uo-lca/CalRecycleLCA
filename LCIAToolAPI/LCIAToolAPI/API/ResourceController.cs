using System;
using System.Collections.Generic;
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

        [Route("api/flowTypes")]
        [HttpGet]
        public IEnumerable<FlowTypeResource> GetFlowTypes() {
            return _ResourceService.GetFlowTypes();
        }

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

        [Route("api/fragments")]
        [HttpGet]
        public IEnumerable<FragmentResource> GetFragments() {
            return _ResourceService.GetFragmentResources();
        }

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
        [Route("api/fragments/{fragmentID:int}/fragmentflows")]
        [Route("api/scenarios/{scenarioID:int}/fragments/{fragmentID:int}/fragmentflows")]
        [HttpGet]
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = 0) {
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
