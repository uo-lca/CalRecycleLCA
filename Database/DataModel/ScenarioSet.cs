namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScenarioSet")]
    public partial class ScenarioSet
    {
        public int ScenarioSetID { get; set; }

        [StringLength(30)]
        public string Name { get; set; }
    }
}
