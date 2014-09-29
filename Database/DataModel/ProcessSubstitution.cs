namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcessSubstitution")]
    public partial class ProcessSubstitution : Entity
    {
        [Key]
        [Column(Order = 1)]
        public int FragmentNodeProcessID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ScenarioID { get; set; }
        
        public int ProcessID { get; set; }

        // Navigation Properties
        public virtual FragmentNodeProcess FragmentNodeProcess { get; set; }
        public virtual Scenario Scenario { get; set; }
        public virtual Process Process  { get; set; }
        
    }
}
