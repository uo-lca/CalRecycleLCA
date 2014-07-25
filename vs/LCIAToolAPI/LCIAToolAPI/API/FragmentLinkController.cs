using Entities.Models;
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
    public class FragmentLinkController : ApiController
    {
        [Inject]
        private readonly IFragmentLinkService _fragmentLinkService ;


        public FragmentLinkController(IFragmentLinkService fragmentLinkService)
        {

            if (fragmentLinkService == null)
            {
                throw new ArgumentNullException("fragmentLink is null");
            }

            _fragmentLinkService = fragmentLinkService;

        }

        [Route("api/fragments/{fragmentID:int}/links")]
        [System.Web.Http.HttpGet]
        public IEnumerable<FragmentLink> GetFragmentLinks(int fragmentID) {
            // Default to base scenario
            return _fragmentLinkService.GetFragmentLinks(fragmentID, 1);
        }

        [Route("api/fragments/{fragmentID:int}/scenarios/{scenarioID:int}/links")]
        [System.Web.Http.HttpGet]
        public IEnumerable<FragmentLink> GetFragmentLinksForScenario( int fragmentID, int scenarioID )
        {
            return _fragmentLinkService.GetFragmentLinks(fragmentID, scenarioID);
        }

    }
}