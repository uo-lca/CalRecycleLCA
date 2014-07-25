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
    public class FlowController : ApiController
    {
        [Inject]
        private readonly IFlowService _flowService ;


        public FlowController(IFlowService flowService)
        {

            if (flowService == null)
            {
                throw new ArgumentNullException("fragmentLink is null");
            }

            _flowService = flowService;

        }

        [Route("api/fragments/{fragmentID:int}/flows")]
        [System.Web.Http.HttpGet]
        public IEnumerable<FlowModel> GetFlows(int fragmentID) {
            return _flowService.GetFlowsByFragment(fragmentID);
        }

    }
}