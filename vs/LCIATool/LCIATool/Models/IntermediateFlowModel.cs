using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class IntermediateFlowModel
    {
        public string FlowPropertyName { get; set; }



        public string ReferenceUnit { get; set; }
        public string FlowType { get; set; }
        public int? ProcessFlowID { get; set; }
        public int? FlowDirectionID { get; set; }
        public int? FlowPropertyID { get; set; }
        public double? ProcessFlowResult { get; set; }
        public double? Computation { get; set; }
        [JsonIgnore]
        public string FlowName { get; set; }

      
        public string FlowDirection { get; set; }

        public int? ProcessID { get; set; }
        public double? IntermediateFlowSum { get; set; }
    }
}