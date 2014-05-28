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
    public class LCIAComputationController : ApiController
    {
        // GET api/<controller>
        static IRepository repository = new Repository();

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IQueryable<LCIAComputationModel> LCIAComputation()
        {
            int processID = 0;
            int lciaMethodID = 0;
            int impactCategoryID = 0;

            //grab the values from the querystring and assign each to a local variable
            if (HttpContext.Current.Request.QueryString["processID"] != null)
            {
                processID = Convert.ToInt32(HttpContext.Current.Request.QueryString["processID"].ToString());
            }

            if (HttpContext.Current.Request.QueryString["lciaMethodId"] != null)
            {
                lciaMethodID = Convert.ToInt32(HttpContext.Current.Request.QueryString["lciaMethodId"].ToString());
            }

            if (HttpContext.Current.Request.QueryString["impactCategoryId"] != null)
            {
                impactCategoryID = Convert.ToInt32(HttpContext.Current.Request.QueryString["impactCategoryId"].ToString());
            }

            //We return the records which correspond to what is sent in the querystring of the api call.
            //if the parameter is not sent for any of the above omit it from the query.
            var _lciaList = repository.LCIAComputation()
                 .Where(l => (l.ProcessID== processID || processID == 0) && (l.LCIAMethodID == lciaMethodID || lciaMethodID == 0) && (l.ImpactCategoryID == impactCategoryID || impactCategoryID == 0));
            return _lciaList;
        }

        
    }
}