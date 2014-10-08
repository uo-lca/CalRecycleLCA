using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class LCIAModel
    {
        public int? LCIAID { get; set; }
        public int? FlowID { get; set; }
        public double? Value { get; set; }
        public int? ParamID { get; set; }
        public int? ScenarioID { get; set; }
        public int? LCIAMethodID { get; set; }
        public int? DirectionID { get; set; }
        public string Geography { get; set; }
        public double? Result { get; set; }
        public double? LCParamValue { get; set; }
        public double? LCIAFactor { get; set; }
        public double? ComputationResult { get; set; }
        public double? Score { get; set; }
    }
}
