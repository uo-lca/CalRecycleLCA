using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LCIATest
{
    public class GWPtController : ApiController
    {
        static IGWPtRepository repository = new GWPtRepository();

        public IEnumerable<GWPt> GetAllData()
        {
            var gwtp = repository.GetAllData();
            return gwtp;
        }
    }
}