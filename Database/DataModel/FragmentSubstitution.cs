namespace LcaDataModel
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentSubstitution")]
    public partial class FragmentSubstitution : Entity
    {
        [Key]
        [Column(Order = 1)]
        public int FragmentNodeFragmentID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ScenarioID { get; set; }
        
        public int SubFragmentID { get; set; }

        // Navigation Properties
        public virtual FragmentNodeFragment FragmentNodeFragment { get; set; }
        public virtual Scenario Scenario { get; set; }
        public virtual Fragment SubFragment { get; set; }
        
    }
}
