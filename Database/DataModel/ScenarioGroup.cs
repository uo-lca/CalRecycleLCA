namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScenarioGroup")]
    public partial class ScenarioGroup
    {
        public ScenarioGroup()
        {
            Scenarios = new HashSet<Scenario>();
        }

        public int ScenarioGroupID { get; set; }

        public int? OwnerID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Visibility { get; set; }

        public virtual ICollection<Scenario> Scenarios { get; set; }

        public virtual User User { get; set; }
    }
}
