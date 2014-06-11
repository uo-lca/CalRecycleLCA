namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScenarioParam")]
    public partial class ScenarioParam
    {
        public int ScenarioParamID { get; set; }

        public int? ScenarioID { get; set; }

        public int? ParamID { get; set; }

        public double? Value { get; set; }

        public virtual Param Param { get; set; }
    }
}
