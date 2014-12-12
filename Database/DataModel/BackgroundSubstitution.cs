namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BackgroundSubstitution")]
    public partial class BackgroundSubstitution : Entity
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BackgroundSubstitutionID { get; set; }

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
