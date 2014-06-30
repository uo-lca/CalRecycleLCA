namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DistributionParam")]
    public partial class DistributionParam
    {
        public int DistributionParamID { get; set; }

        public int? DependencyParamID { get; set; }

        public int? ConservationParamID { get; set; }

        public virtual DependencyParam DependencyParam { get; set; }

        public virtual DependencyParam DependencyParam1 { get; set; }
    }
}
