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
        int processID = 0;
        int lciaMethodId = 0;
        int impactCategoryId = 0;

        // GET api/<controller>
        static IRepository repository = new Repository();

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IQueryable<LCIAComputationModel> LCIAComputation()
        {
            if (HttpContext.Current.Request.QueryString["processID"] != null)
            {
                processID = Convert.ToInt32(HttpContext.Current.Request.QueryString["processID"].ToString());
            }

            if (HttpContext.Current.Request.QueryString["lciaMethodId"] != null)
            {
                lciaMethodId = Convert.ToInt32(HttpContext.Current.Request.QueryString["lciaMethodId"].ToString());
            }

            if (HttpContext.Current.Request.QueryString["impactCategoryId"] != null)
            {
                impactCategoryId = Convert.ToInt32(HttpContext.Current.Request.QueryString["impactCategoryId"].ToString());
            }


            var _lciaList = repository.LCIAComputation()
                 .Where(l => l.FlowTypeID == 2 && (l.ProcessID== processID || processID == 0) && (l.LCIAMethodID == lciaMethodId || lciaMethodId == 0) && (l.ImpactCategoryID == impactCategoryId || impactCategoryId == 0));
            return _lciaList;

        }
    }
}