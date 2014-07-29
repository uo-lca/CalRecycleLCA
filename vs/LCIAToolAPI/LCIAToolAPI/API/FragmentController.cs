using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repository;
using Services;
using LcaDataModel;
using Entities.Models;
using System.Web.Http.Services;
using Ninject;
using LCAToolAPI.Infrastructure;
using Ninject.Extensions.MissingBindingLogger;
using log4net;
using System.Reflection;
using Ninject.Extensions.Logging;
using LCAToolAPI;


namespace LCIAToolAPI.API
{

    public class FragmentController : ApiController
    {

        [Inject]
        private readonly IFragmentService _fragmentService;

        public FragmentController(IFragmentService fragmentService)
        {

            if (fragmentService == null)
            {
                throw new ArgumentNullException("fragmentService is null");
            }

            _fragmentService = fragmentService;

        }

        [Route("api/fragments")]
        [System.Web.Http.HttpGet]
        public IEnumerable<FragmentModel> GetFragments()      
        {
            return _fragmentService.GetFragments();
        }

        [Route("api/fragments/{fragmentID:int}")]
        [System.Web.Http.HttpGet]
        public FragmentModel GetFragment(int fragmentID) {
            return _fragmentService.GetFragment(fragmentID);
        }


    }
}