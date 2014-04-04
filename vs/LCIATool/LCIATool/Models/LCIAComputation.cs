using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class LCIAComputation
    {
        public IEnumerable<Process> Processes { get; set; }
        public IEnumerable<ProcessFlow> ProcessFlows { get; set; }
        public IEnumerable<> Flows { get; set; }
    }
}