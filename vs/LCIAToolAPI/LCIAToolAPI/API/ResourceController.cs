using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ninject;
using Services;
using Entities.Models;

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

        [Route("api/fragments/{fragmentID:int}/fragmentflows")]
        [HttpGet]
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID) {
            // Use default scenario
            return _ResourceService.GetFragmentFlowResources(fragmentID, 1);
        }

        [Route("api/fragments/{fragmentID:int}/scenarios/{scenarioID:int}/fragmentflows")]
        [HttpGet]
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID) {
            return _ResourceService.GetFragmentFlowResources(fragmentID, scenarioID);
        }

        [Route("api/fragments/{fragmentID:int}/flows")]
        [HttpGet]
        public IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID) {
            return _ResourceService.GetFlowsByFragment(fragmentID);
        }

        [Route("api/fragments/{fragmentID:int}/flowproperties")]
        [HttpGet]
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID) {
            return _ResourceService.GetFlowPropertiesByFragment(fragmentID);
        }

        [Route("api/impactcategories")]
        [HttpGet]
        public IEnumerable<ImpactCategoryResource> ImpactCategories() {
            return _ResourceService.GetImpactCategories();
        }

        [Route("api/impactcategories/{impactCategoryID:int}/lciamethods")]
        [HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodsByImpactCategory(int impactCategoryID) {
            return _ResourceService.GetLCIAMethodResources(impactCategoryID);
        }

        [Route("api/lciamethods")]
        [HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodResources() {
            return _ResourceService.GetLCIAMethodResources();
        }

        [Route("api/processes")]
        [HttpGet]
        public IEnumerable<ProcessResource> GetProcesses() {
            return _ResourceService.GetProcesses();
        }

        [Route("api/flowtypes/{flowTypeID:int}/processes")]
        [HttpGet]
        public IEnumerable<ProcessResource> GetProcesses(int flowTypeID) {
            return _ResourceService.GetProcesses(flowTypeID);
        }

        [Route("api/processes/{processID:int}/flows")]
        [HttpGet]
        public IEnumerable<FlowResource> GetFlowsByProcess(int processID) {
            return _ResourceService.GetFlowsByProcess(processID);
        }

        [Route("api/processes/{processID:int}/flowproperties")]
        [HttpGet]
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID) {
            return _ResourceService.GetFlowPropertiesByProcess(processID);
        }

        [Route("api/processes/{processID:int}/lciamethods/{lciaMethodID:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetLCIAResultResources(int processID, int lciaMethodID) {
            return _ResourceService.GetLCIAResultResources(processID, lciaMethodID);
        }

        [Route("api/processes/{processID:int}/lciamethods/{lciaMethodID:int}/scenarios/{scenarioID:int}/lciaresults")]
        [HttpGet]
        public IEnumerable<LCIAResultResource> GetLCIAResultResources(int processID, int lciaMethodID, int scenarioID) {
            return _ResourceService.GetLCIAResultResources(processID, lciaMethodID, scenarioID);
        }

        /* TODO:
         * 
         * [Route("api/processes/{processID:int}")]
         * [Route("api/fragments/{fragmentID:int}/lciaresults")]    optional scenario parameter
         * 
         */
    }
}
