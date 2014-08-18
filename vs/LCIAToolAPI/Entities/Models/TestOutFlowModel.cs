using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class TestOutFlowModel
    {
        public int? FlowID { get; set; }
        public int? FFDirectionID { get; set; }
        public int? NFDirectionID { get; set; }
        public double? Result { get; set; }
        public double? FlowMagnitude { get; set; }
        public int? RefFlowID { get; set; }
        public int? FragmentID { get; set; }
        public int? ScenarioID { get; set; }
        public int? FragmentFlowID { get; set; }
        public int? ParamID { get; set; }
        public int? ParentFragmentFlowID { get; set; }
        public double? ParamValue { get; set; }
    }
}
