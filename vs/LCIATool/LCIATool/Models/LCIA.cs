using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class LCIA
    {
        public int LCAID { get; set; }
        public int LCIAMethodID { get; set; }
        public int LCIAFlowID { get; set; }
        public int LCIADirectionID { get; set; }
        public float Factor { get; set; }
    }
}