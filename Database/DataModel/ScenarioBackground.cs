namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScenarioBackground")]
    public partial class ScenarioBackground : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScenarioBackgroundID { get; set; }

        public int ScenarioID { get; set; }

        public int FlowID { get; set; }

        public int DirectionID { get; set; }

        public int NodeTypeID { get; set; }

        public int? ILCDEntityID { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual NodeType NodeType { get; set; }

        public virtual Scenario Scenario { get; set; }
    }
}
