namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentEdge")]
    public partial class FragmentEdge
    {
        public int FragmentEdgeID { get; set; }

        public int? FragmentID { get; set; }

        public int? FragmentNodeID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        public int? Terminus { get; set; }

        public int? ScenarioID { get; set; }

        public int? ParamID { get; set; }

        public double? Quantity { get; set; }
    }
}
