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
    public class LCIAMethodController : ApiController
    {
        
        // GET api/<controller>
        static IRepository repository = new Repository();
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IQueryable<LCIAMethodModel> LCIAMethodDDL()
        {
            int impactCategoryID = 0;

            //grab the impactCategoryid from the querystring and assign them to a local variable
            if (HttpContext.Current.Request.QueryString["impactCategoryid"] != null)
            {
            impactCategoryID = Convert.ToInt32(HttpContext.Current.Request.QueryString["impactCategoryid"].ToString());
            }

            if (impactCategoryID == 0)
            {
                //if there is no impactCategoryid parameter in the querystring we return all LCIAMethods
                var lciaMethods = repository.LCIAMethodDDL();
                return lciaMethods;
            }
            else
            {
                //We return the LCIA methods which correspond to what is sent in the querystring of the api call.
                var lciaMethods = repository.LCIAMethodDDL()
                     .Where(lm => lm.ImpactCategoryID == impactCategoryID);
                return lciaMethods;

            }
        }
    }
}