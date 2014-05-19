using LCIATool.Models;
using LCIATool.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace LCIATool.API
{
    public class IntermediateFlowSumController : ApiController
    {
        static IRepository repository = new Repository();

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public double IntermediateFlowSum()
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
            var intermediateFlowSum = repository.IntermediateFlowSum(balance, processId);
            return intermediateFlowSum;
        }
    }
}