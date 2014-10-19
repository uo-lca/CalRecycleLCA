namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
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
            DataSources = new HashSet<DataSource>();
        }

        public int VisibilityID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<ScenarioGroup> ScenarioGroups { get; set; }
        public virtual ICollection<DataSource> DataSources { get; set; }
    }
}
