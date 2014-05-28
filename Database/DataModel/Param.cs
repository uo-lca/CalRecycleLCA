namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Param")]
    public partial class Param
    {
        public int ParamID { get; set; }

        public int? ScenarioSetID { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        public int? FlowID { get; set; }

        public double? ParamDefault { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public double? Scale { get; set; }
    }
}
