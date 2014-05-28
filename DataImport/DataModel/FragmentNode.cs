namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentNode")]
    public partial class FragmentNode
    {
        public int FragmentNodeID { get; set; }

        public int? FragmentID { get; set; }

        public int? ProcessID { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        public int? StageID { get; set; }

        public int? ScenarioID { get; set; }

        public int? ParamID { get; set; }

        public double? Weight { get; set; }
    }
}
