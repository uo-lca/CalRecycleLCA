namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompositionData")]
    public partial class CompositionData : Entity
    {
        public int CompositionDataID { get; set; }

        public int CompositionModelID { get; set; }

        public int FlowPropertyID { get; set; }

        public double Value { get; set; }

        public virtual CompositionModel CompositionModel { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }
    }
}
