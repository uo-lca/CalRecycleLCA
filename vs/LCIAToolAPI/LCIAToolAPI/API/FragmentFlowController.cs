using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repository;
using Services;
using Data;
using Entities.Models;
using System.Web.Http.Services;
using Ninject;
using LCAToolAPI.Infrastructure;
using Ninject.Extensions.MissingBindingLogger;
using log4net;
using System.Reflection;
using Ninject.Extensions.Logging;

namespace LCAToolAPI.API
{
    public class FragmentFlowController : ApiController
    {
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;

        public FragmentFlowController(IFragmentFlowService fragmentFlowService)
        {

            if (fragmentFlowService == null)
            {
                throw new ArgumentNullException("fragmentFlowService is null");
            }

            _fragmentFlowService = fragmentFlowService;


        }

        [Route("api/fragments/{fragmentID}/fragmentflows")]
        [System.Web.Http.HttpGet]
        public IEnumerable<FragmentFlow> GetFragmentFlows(int fragmentID)
        
        {
            return _fragmentFlowService.GetFragmentFlows(fragmentID);
        }
    }
}
