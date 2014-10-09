﻿
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
        [Inject]
        private readonly IFragmentLCIAComputation _fragmentLCIAComputation;


        public LCIAComputationController(ILCIAComputationV2 lciaComputationV2, IFragmentLCIAComputation fragmentLCIAComputation)
        {

            if (lciaComputationV2 == null)
            {
                throw new ArgumentNullException("lciaComputationV2 is null");
            }

            _lciaComputationV2 = lciaComputationV2;

            if (fragmentLCIAComputation == null)
            {
                throw new ArgumentNullException("fragmentLCIAComputation is null");
            }

            _fragmentLCIAComputation = fragmentLCIAComputation;

        }

        //GET api/<controller>
         [Route("api/processes/{ProcessID}/scenarios/{scenarioID}/compute")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<LCIAModel> LCIACompute(int processId, int scenarioId)
        {
            return _lciaComputationV2.LCIACompute(processId, scenarioId);
        }

         [Route("api/fragments/{FragmentID}/scenarios/{scenarioID}/compute")]
         [System.Web.Http.AcceptVerbs("GET", "POST")]
         [System.Web.Http.HttpGet]
         public void LCIAFragmentCompute(int fragmentId, int scenarioId)
         {
            _fragmentLCIAComputation.FragmentLCIACompute(fragmentId, scenarioId);
         }


       
    }
}