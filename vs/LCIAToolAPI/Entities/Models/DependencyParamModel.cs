using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class DependencyParamModel
    {
        public int ScenarioParamID { get; set; }

        public int? ScenarioID { get; set; }

        public int? ParamID { get; set; }

        public int DependencyParamID { get; set; }

        public int? FragmentFlowID { get; set; }

        public double? Value { get; set; }
    }
}
