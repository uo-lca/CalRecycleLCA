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
        private readonly IResourceService _ResourceService;

        public ResourceController(ResourceService resourceService)
        {
            if (resourceService == null)
            {
                throw new ArgumentNullException("resourceService");
            }
            _ResourceService = resourceService;
        }

        [Route("api/lciamethods")]
        [System.Web.Http.HttpGet]
        public IEnumerable<LCIAMethodResource> GetLCIAMethodResources()
        {
            return _ResourceService.GetLCIAMethodResources();
        }
    }
}
