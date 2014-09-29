namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Visibility")]
    public partial class Visibility : Entity
    {
        public Visibility()
        {
            ScenarioGroups = new HashSet<ScenarioGroup>();
        }

        public int VisibilityID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<ScenarioGroup> ScenarioGroups { get; set; }
    }
}
