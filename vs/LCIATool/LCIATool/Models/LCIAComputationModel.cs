using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models
{
    public class LCIAComputationModel
    {
        public string ProcessName { get; set; }
        public int FlowID { get; set; }
        [JsonIgnore]
        public int FlowTypeID { get; set; }
        [JsonIgnore]
        public int ProcessID { get; set; }
        [JsonIgnore]
        public int LCIAMethodID { get; set; }
        [JsonIgnore]
        public int? ImpactCategoryID { get; set; }
        public string Flow { get; set; }
        public string Direction { get; set; }
        public double? Quantity { get; set; }
        public double? STDev { get; set; }
        public double? Factor { get; set; }
        public double? LCIAResult { get; set; }
    }
}