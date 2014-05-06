using LCIATool.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LCIATool.Models;

namespace LCIATool.API
{
    public class CountController : ApiController
    {
        static IRepository repository = new Repository();
        // GET api/<controller>
        public int Get()
        {
            IQueryable<LCIAComputationModel> list = repository.LCIAComputation();
            int cnt = list.Count();
            return cnt;
           
        }

    }
}