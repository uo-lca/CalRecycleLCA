namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowPropertyEmission")]
    public partial class FlowPropertyEmission
    {
        public int FlowPropertyEmissionID { get; set; }

        public int? FlowPropertyID { get; set; }

        public int? EmissionID { get; set; }

        public double? Scale { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }
    }
}
