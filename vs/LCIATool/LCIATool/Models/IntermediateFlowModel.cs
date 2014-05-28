using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class IntermediateFlowModel
    {
        public string ReferenceProperty { get; set; }



        public string ReferenceUnit { get; set; }

        [JsonIgnore]
        public string FlowType { get; set; }
        public int? ProcessFlowID { get; set; }
        [JsonIgnore]
        public int? FlowTypeID { get; set; }
        public int? FlowDirectionID { get; set; }
        public int? FlowPropertyID { get; set; }
        public double? Quantity { get; set; }
        public double? NetFlowIn { get; set; }
        
        public string FlowName { get; set; }

      
        public string FlowDirection { get; set; }

        public int? ProcessID { get; set; }
        public double? IntermediateFlowSum { get; set; }

        [JsonIgnore]
        public double? MaxUnit { get; set; }
        
        public double? SankeyWidth { get; set; }
    }
}