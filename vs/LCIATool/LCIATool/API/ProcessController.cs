using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LCIATool.Models.Repository;
using LCIATool.Models;

namespace LCIATool.API
{
    public class ProcessController : ApiController
    {
        // GET api/<controller>

         static IRepository repository = new Repository();
         [System.Web.Http.AcceptVerbs("GET", "POST")]
         [System.Web.Http.HttpGet]
         public IQueryable<ProcessModel> ProcessDDL(int flows = 0)
         {
             //gets all processes for the dropdownlist by calling the function within the interface.
             var processes = repository.ProcessDDL(flows);
             return processes;
         }
    }
}