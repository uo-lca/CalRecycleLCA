namespace Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScenarioParam")]
    public partial class ScenarioParam
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScenarioParamID { get; set; }

        public int? ScenarioID { get; set; }

        public int? ParamID { get; set; }

        public virtual Param Param { get; set; }

        public virtual Scenario Scenario { get; set; }
    }
}
