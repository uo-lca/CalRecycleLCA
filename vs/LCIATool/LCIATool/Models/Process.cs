using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class Process
    {
        public int ProcessID { get; set; }
        public int ProcessUUID { get; set; }
        public int ProcessVersion { get; set; }
        public string ProcessName { get; set; }
        public DateTime Year { get; set; }
        public string Geography { get; set; }
        public int ReferenceFlow { get; set; }
        public string RefererenceType { get; set; }
        public string ProcessType { get; set; }
        public string Diagram { get; set; }

    }
}