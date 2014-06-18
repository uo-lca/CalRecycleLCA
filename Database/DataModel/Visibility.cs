namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Visibility")]
    public partial class Visibility
    {
        public Visibility()
        {
            ScenarioGroups = new HashSet<ScenarioGroup>();
        }

        public int VisibilityID { get; set; }

        [Column("Visibility")]
        [StringLength(50)]
        public string Visibility1 { get; set; }

        public virtual ICollection<ScenarioGroup> ScenarioGroups { get; set; }
    }
}
