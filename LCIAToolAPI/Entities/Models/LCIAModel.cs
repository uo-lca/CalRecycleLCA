using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class LCIAModel
    {
        public int? ScenarioID { get; set; }
        public int? LCIAMethodID { get; set; }
        public int FlowID { get; set; }
        public int DirectionID { get; set; }
        public double? Quantity { get; set; }
        public double? Factor { get; set; }
        public double? LCIAResult { get; set; }
        public ParamInstance CharacterizationParam { get; set; }
        public string Geography { get; set; }
    }
}
