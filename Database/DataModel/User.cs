namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("User")]
    public partial class User : Entity
    {
        public const int AUTHENTICATED_USER = 2;

        public User()
        {
            ScenarioGroups = new HashSet<ScenarioGroup>();
        }

        public int UserID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public bool? CanLogin { get; set; }

        public bool? CanEditScenarios { get; set; }

        public bool? CanEditFragments { get; set; }

        public bool? CanEditBackground { get; set; }

        public bool? CanAppend { get; set; }

        public virtual ICollection<ScenarioGroup> ScenarioGroups { get; set; }
    }
}
