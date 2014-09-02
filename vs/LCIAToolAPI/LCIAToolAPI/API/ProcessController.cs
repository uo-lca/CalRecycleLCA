using Entities.Models;
using LcaDataModel;
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
        //[Inject]
        //private readonly IProcessService _processService ;


        //public ProcessController(IProcessService processService)
        //{

        //    if (processService == null)
        //    {
        //        throw new ArgumentNullException("processService is null");
        //    }

        //    _processService = processService;

        //}

        private readonly IService<Process> _processService;

        public ProcessController(IService<Process> processService)
        {
            _processService = processService;
        }

        //[Route("api/processes")]
        //[System.Web.Http.HttpGet]
        //public IEnumerable<ProcessModel> GetProcesses() {
        //    return _processService.GetProcesses();

        [Route("api/processes")]
        [System.Web.Http.HttpGet]
        public IEnumerable<Process> GetProcesses()
        {
            return _processService.Query().Get();
        }
    }
}