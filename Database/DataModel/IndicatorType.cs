namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IndicatorType")]
    public partial class IndicatorType : Entity
    {
        public IndicatorType()
        {
            LCIAMethods = new HashSet<LCIAMethod>();
        }

        public int IndicatorTypeID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }
    }
}
