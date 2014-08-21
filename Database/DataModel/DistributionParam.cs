namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DistributionParam")]
    public partial class DistributionParam
    {
        [Key]
        public int DependencyParamID { get; set; }

        public int ConservationDependencyParamID { get; set; }

        // Navigation properties
        public virtual DependencyParam DependencyParam { get; set; }
        public virtual DependencyParam ConservationDependencyParam { get; set; }
    }
}
