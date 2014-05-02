using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class ProcessFlowModel
    {
        public int ProcessFlowID { get; set; }
        public int ProcessFlowProcessID { get; set; }
        public int FlowID { get; set; }
        public int DirectionID { get; set; }
        public string Type { get; set; }
        public string VarName { get; set; }
        public float Magnitude { get; set; }
        public float Result { get; set; }
        public float STDev { get; set; }
    }
}