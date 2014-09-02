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
    public class ProcessController : ApiController
    {
        [Inject]
        private readonly IProcessService _processService ;


        public ProcessController(IProcessService processService)
        {

            if (processService == null)
            {
                throw new ArgumentNullException("processService is null");
            }

            _processService = processService;

        }

        [Route("api/processes")]
        [System.Web.Http.HttpGet]
        public IEnumerable<ProcessModel> GetProcesses() {
            return _processService.GetProcesses();
        }
    }
}