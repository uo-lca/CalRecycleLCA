using LcaDataModel;
using Ninject;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CalRecycleLCA.Services;
using Entities.Models;

namespace LCAToolAPI.API
{
    /// <summary>
    /// Internal API routes for testing and debugging Fragment Traversal.
    /// </summary>
    public class FragmentTraversalController : ApiController
    {

        [Inject]
        private readonly IFragmentLCIAComputation _fragmentLciaComputation;

        /// <summary>
        /// Constructor for Fragment traversal diagnostic controller.
        /// Assigns a local private IFragmentTraversalV2 object via dependency injection.
        /// </summary>
        /// <param name="fragmentLciaComputation">via dependency injection</param>
        public FragmentTraversalController(IFragmentLCIAComputation fragmentLciaComputation)
        {
            if (fragmentLciaComputation == null)
            {
                throw new ArgumentNullException("fragmentTraversalV2 is null");
            }

            _fragmentLciaComputation = fragmentLciaComputation;

        }

        //GET api/<controller>
        /// <summary>
        /// Commands the back-end code to traverse the named fragment and scenario and store
        /// the results to the NodeCache.  If the fragment has already been traversed (i.e.
        /// the reference node already has a NodeCache entry for the given scenario), then
        /// nothing happens.
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="scenarioID"></param>
        [Route("api/fragments/{fragmentID}/scenarios/{scenarioID}/traverse")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IEnumerable<NodeCache> Traversal(int fragmentID, int scenarioID)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var nodeCaches = _fragmentLciaComputation.FragmentTraverse(fragmentID, scenarioID);
            sw.Stop();
            return nodeCaches;
        }

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}

    }
}