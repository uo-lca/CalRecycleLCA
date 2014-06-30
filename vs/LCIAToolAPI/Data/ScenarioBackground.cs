namespace Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScenarioBackground")]
    public partial class ScenarioBackground
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScenarioBackgroundID { get; set; }

        public int? ScenarioID { get; set; }

        public int? FlowID { get; set; }

        public int? NodeTypeID { get; set; }

        public int? TargetID { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual NodeType NodeType { get; set; }

        public virtual Scenario Scenario { get; set; }
    }
}
