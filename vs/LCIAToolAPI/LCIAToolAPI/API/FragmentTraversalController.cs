using Data;
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
    public class FragmentTraversalController : ApiController
    {
        [Inject]
        private readonly IFragmentTraversal _fragmentTraversal ;


        public FragmentTraversalController(IFragmentTraversal fragmentTraversal)
        {

            if (fragmentTraversal == null)
            {
                throw new ArgumentNullException("fragmentTraversal is null");
            }

            _fragmentTraversal = fragmentTraversal;

        }

        //int scenarioId = 1;

        //GET api/<controller>
        [Route("api/fragments/{fragmentID}/scenarios/{scenarioID}/traverse")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public void Traversal( int fragmentID, int scenarioID )
        {
            _fragmentTraversal.Traverse(fragmentID, scenarioID);
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