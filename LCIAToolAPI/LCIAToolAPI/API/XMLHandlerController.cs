
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
    /// <summary>
    /// this is ultimately supposed to provide access to the XML files (except for processes that are protected).
    /// </summary>
    public class XMLHandlerController : ApiController
    {
        [Inject]
        private readonly IFlowTestService _flowTestService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTestService"></param>
        public XMLHandlerController(IFlowTestService flowTestService)
        {
            if (flowTestService == null)
            {
                throw new ArgumentNullException("flowTestService is null");
            }

            _flowTestService = flowTestService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("api/xmlhandler")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public XmlDocument FlowTest()
        {
            return _flowTestService.ViewXML();
        }
    }
}
