using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LCIATool.Models.Repository;
using LCIATool.Models;
using System.Web;

namespace LCIATool.API
{
    public class IntermediateFlowController : ApiController
    {
        static IRepository repository = new Repository();

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IQueryable<IntermediateFlowModel> IntermediateFlow()
        {
            int balance = 0;
            int processId = 0;

            //grab the values from the querystring and assign each to a local variable
            if (HttpContext.Current.Request.QueryString["processId"] != null)
            {
                processId = Convert.ToInt32(HttpContext.Current.Request.QueryString["processId"].ToString());
            }

            if (HttpContext.Current.Request.QueryString["balance"] != null)
            {
                balance = Convert.ToInt32(HttpContext.Current.Request.QueryString["balance"].ToString());
            }

            //We return the records which correspond to what is sent in the querystring of the api call.
            //if the parameter is not sent for any of the above omit it from the query.
            var intermediateFlows = repository.IntermediateFlow(balance, processId);
            return intermediateFlows;
        }
        
    }
}