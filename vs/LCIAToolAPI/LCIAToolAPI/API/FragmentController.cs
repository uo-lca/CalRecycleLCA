using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repository;
using Services;
using Data.Mappings;
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

        //private readonly IFragmentService _fragmentService;
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




        // GET api/<controller>
        //[System.Web.Http.AcceptVerbs("GET", "POST")]
        //[System.Web.Http.HttpGet]
        //public IEnumerable<Fragment> ImpactCategoryDDL()
        //{

        //    var processes = _fragmentService.GetFragments();
        //    return processes;


        //}

      


    }
}