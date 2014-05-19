using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class ProcessModel
    {

        public int? ProcessID { get; set; }
        public string ProcessUUID { get; set; }
        public string ProcessVersion { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string Geography { get; set; }
        public string ReferenceFlow_SQL { get; set; }
        public string RefererenceType { get; set; }
        public string ProcessType { get; set; }
        public string Diagram { get; set; }
        public Nullable<int> FlowID { get; set; }

    }
}