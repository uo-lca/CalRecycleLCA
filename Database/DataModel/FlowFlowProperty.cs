namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowFlowProperty")]
    public partial class FlowFlowProperty : Entity
    {
        public FlowFlowProperty()
        {
            FlowPropertyParams = new HashSet<FlowPropertyParam>();
        }

        public int FlowFlowPropertyID { get; set; }

        public int FlowID { get; set; }

        public int FlowPropertyID { get; set; }

        public double MeanValue { get; set; }

        public double? StDev { get; set; }


        public virtual Flow Flow { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }

        public virtual ICollection<FlowPropertyParam> FlowPropertyParams { get; set; }
    }
}
