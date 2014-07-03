using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Entities.Models
{
    public class DependencyParamModel
    {
        public int FragmentFlowID { get; set; }

        public int? FragmentID { get; set; }

        public string Name { get; set; }

        public int? FragmentStageID { get; set; }

        public int? ReferenceFlowPropertyID { get; set; }

        public int? NodeTypeID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public double? Quantity { get; set; }

        public int? ParentFragmentFlowID { get; set; }

        [JsonIgnore]
        public int DependencyParamID { get; set; }

        [JsonIgnore]
        public int? ParamID { get; set; }

        public double? Value { get; set; }
    }
}
