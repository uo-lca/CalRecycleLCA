using LcaDataModel;
using Ninject;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace LCAToolAPI.API
{
    public class TestGenericServiceController : ApiController
    {
      
        [Inject]
        private readonly ITestGenericService _fragmentTraversalV2;


        public TestGenericServiceController(ITestGenericService fragmentTraversalV2)
        {
            if (fragmentTraversalV2 == null)
            {
                throw new ArgumentNullException("fragmentTraversalV2 is null");
            }

            _fragmentTraversalV2 = fragmentTraversalV2;

        }

        //GET api/<controller>
        [Route("api/generic")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<LCIAMethod> Test()
        {
            return _fragmentTraversalV2.GetLCIAMethods();
        }
    }
}