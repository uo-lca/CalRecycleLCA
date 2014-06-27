namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Scenario")]
    public partial class Scenario
    {
        public Scenario()
        {
            ScenarioBackgrounds = new HashSet<ScenarioBackground>();
        }

        public int ScenarioID { get; set; }

        public int? ScenarioGroupID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual ScenarioGroup ScenarioGroup { get; set; }

        public virtual ICollection<ScenarioBackground> ScenarioBackgrounds { get; set; }
    }
}
