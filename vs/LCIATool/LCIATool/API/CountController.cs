using LCIATool.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LCIATool.Models;
using System.Web;

namespace LCIATool.API
{
    public class CountController : ApiController
    {
        static IRepository repository = new Repository();
        private LCAToolDevEntities1 context = new LCAToolDevEntities1();
        // GET api/<controller>
        public int Get()
        {
            //IQueryable<LCIAComputationModel> list = repository.LCIAComputation();
            //int cnt = list.Count();
            //return cnt;
            int balance = 0;
            int processId = 0;

            //grab the values from the querystring and assign each to a local variable
            if (HttpContext.Current.Request.QueryString["processID"] != null)
            {
                processId = Convert.ToInt32(HttpContext.Current.Request.QueryString["processID"].ToString());
            }

            if (HttpContext.Current.Request.QueryString["impactCategoryId"] != null)
            {
                balance = Convert.ToInt32(HttpContext.Current.Request.QueryString["balance"].ToString());
            }

            IQueryable<IntermediateFlowModel> list = repository.IntermediateFlow(balance, processId);
            int cnt = list.Count();
            return cnt;

           
        }

    }
}