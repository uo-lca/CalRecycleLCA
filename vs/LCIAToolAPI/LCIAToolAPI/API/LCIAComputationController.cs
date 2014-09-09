
using Entities.Models;
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
    public class LCIAComputationController : ApiController
    {
        [Inject]
        private readonly ILCIAComputationV2 _lciaComputationV2;


        public LCIAComputationController(ILCIAComputationV2 lciaComputationV2)
        {

            if (lciaComputationV2 == null)
            {
                throw new ArgumentNullException("lciaComputationV2 is null");
            }

            _lciaComputationV2 = lciaComputationV2;

        }

        //GET api/<controller>
        [Route("api/compute")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<LCIAModel> Compute()
        {
            return _lciaComputationV2.GetLCIAMethodsForComputeLCIA();
        }

       
    }
}
