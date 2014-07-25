using Entities.Models;
using Ninject;
using Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LCAToolAPI.API
{
    public class FlowPropertyController : ApiController
    {
        [Inject]
        private readonly IFlowPropertyService _flowPropertyService ;


        public FlowPropertyController(IFlowPropertyService flowPropertyService)
        {

            if (flowPropertyService == null)
            {
                throw new ArgumentNullException("flowPropertyService is null");
            }

            _flowPropertyService = flowPropertyService;

        }

        [Route("api/fragments/{fragmentID:int}/flowproperties")]
        [System.Web.Http.HttpGet]
        public IEnumerable<FlowPropertyModel> GetFlowPropertiesByFragment(int fragmentID) {
            return _flowPropertyService.GetFlowPropertiesByFragment(fragmentID);
        }

    }
}