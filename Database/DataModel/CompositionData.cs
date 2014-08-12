namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompositionData")]
    public partial class CompositionData
    {
        public int CompositionDataID { get; set; }

        public int? CompositionModelID { get; set; }

        public int? FlowFlowPropertyID { get; set; }

        public double? Value { get; set; }

        public virtual CompositionModel CompostionModel { get; set; }

        public virtual FlowFlowProperty FlowFlowProperty { get; set; }
    }
}
