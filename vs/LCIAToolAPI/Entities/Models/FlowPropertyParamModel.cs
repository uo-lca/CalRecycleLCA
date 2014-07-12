using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class FlowPropertyParamModel
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

        public double? MeanValue { get; set; }

        public int? FlowFlowPropertyID { get; set; }

        public double? NodeWeight { get; set; }
    }
}
