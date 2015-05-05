using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class InventoryModel
    {
        public int FlowID { get; set; }

        public int DirectionID { get; set; }

        public double? Composition { get; set; } 
        public double? Dissipation { get; set; }
        public double? Result { get; set; }
        public double? StDev { get; set; }

    }

    public class FragmentFlowModel : InventoryModel
    {
        public int? FragmentFlowID { get; set; }
    }

    public class ConservationModel : FragmentFlowModel
    {
        public double? FlowPropertyValue { get; set; }
        public double? FlowPropertyResult
        {
            get { return base.Result * FlowPropertyValue; }
            set { if (FlowPropertyValue != null && FlowPropertyValue != 0)
                    base.Result = value / FlowPropertyValue; }
        }
    }
}
