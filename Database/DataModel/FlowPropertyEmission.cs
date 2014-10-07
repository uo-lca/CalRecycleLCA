namespace LcaDataModel
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowPropertyEmission")]
    public partial class FlowPropertyEmission : Entity
    {
        public int FlowPropertyEmissionID { get; set; }

        public int? FlowPropertyID { get; set; }

        public int? FlowID { get; set; }

        public double? Scale { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }
    }
}
