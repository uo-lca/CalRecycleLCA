namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentEmission")]
    public partial class FragmentEmission
    {
        public int FragmentEmissionID { get; set; }

        public int? NodeID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public int? ScenarioID { get; set; }

        public int? ParamID { get; set; }

        public double? Quantity { get; set; }
    }
}
