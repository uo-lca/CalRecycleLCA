using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace LCIATool.Models.Repository
{
    public class Repository
    {
        private LCAToolDevEntities context = new LCAToolDevEntities();

        public IEnumerable<LCIA> LCIAs
        {
            get { return context.LCIAs;  }
        }

        public IEnumerable<LCIAMethod> LCIAMethods
        {
            get { return context.LCIAMethods; }
        }

        public IEnumerable<Process> Processes
        {
            get { return context.Processes; }
        }
        public IEnumerable<ProcessFlow> ProcessFlows
        {
            get { return context.ProcessFlows; }
        }
    }
}