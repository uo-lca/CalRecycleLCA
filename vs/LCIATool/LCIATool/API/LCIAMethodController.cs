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
        int impactCategoryID = 0;
        // GET api/<controller>
        static IRepository repository = new Repository();
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IQueryable<LCIAMethodModel> LCIAMethodDDL()
        {
            if (HttpContext.Current.Request.QueryString["impactCategoryid"] != null)
            {
            impactCategoryID = Convert.ToInt32(HttpContext.Current.Request.QueryString["impactCategoryid"].ToString());
            }

            if (impactCategoryID == 0)
            {
                var lciaMethods = repository.LCIAMethodDDL();
                return lciaMethods;
            }
            else
            {
                var lciaMethods = repository.LCIAMethodDDL()
                     .Where(lm => lm.ImpactCategoryID == impactCategoryID);
                return lciaMethods;

            }
        }
    }
}