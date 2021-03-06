namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScenarioGroup")]
    public partial class ScenarioGroup : Entity
    {
        public const int BASE_SCENARIO_GROUP = 1;
        
        public ScenarioGroup()
        {
            Scenarios = new HashSet<Scenario>();
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScenarioGroupID { get; set; }

        public int? OwnedBy { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? VisibilityID { get; set; }

        public string Secret { get; set; }

        public virtual ICollection<Scenario> Scenarios { get; set; }

        public virtual User User { get; set; }

        public virtual Visibility Visibility { get; set; }
    }
}
