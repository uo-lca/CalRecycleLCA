
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using XMLHandler;

namespace LCAToolAPI.API
{
    public class XMLHandlerController : ApiController
    {
        [Inject]
        private readonly IFlowTestService _flowTestService;


        public XMLHandlerController(IFlowTestService flowTestService)
        {
            if (flowTestService == null)
            {
                throw new ArgumentNullException("flowTestService is null");
            }

            _flowTestService = flowTestService;
        }

        //GET api/<controller>
        [Route("api/xmlhandler")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public XmlDocument FlowTest()
        {
            return _flowTestService.ViewXML();
        }
    }
}
