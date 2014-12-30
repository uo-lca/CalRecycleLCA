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

namespace LCAToolAPI.API
{
    /// <summary>
    /// Internal API routes for testing and debugging Fragment Traversal.
    /// </summary>
    public class FragmentTraversalController : ApiController
    {

        [Inject]
        private readonly IFragmentTraversalV2 _fragmentTraversalV2;

        /// <summary>
        /// Constructor for Fragment traversal diagnostic controller.
        /// Assigns a local private IFragmentTraversalV2 object via dependency injection.
        /// </summary>
        /// <param name="fragmentTraversalV2">via dependency injection</param>
        public FragmentTraversalController(IFragmentTraversalV2 fragmentTraversalV2)
        {
            if (fragmentTraversalV2 == null)
            {
                throw new ArgumentNullException("fragmentTraversalV2 is null");
            }

            _fragmentTraversalV2 = fragmentTraversalV2;

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
        public void Traversal(int fragmentID, int scenarioID)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _fragmentTraversalV2.Traverse(fragmentID, scenarioID);
            sw.Stop();
            return;
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