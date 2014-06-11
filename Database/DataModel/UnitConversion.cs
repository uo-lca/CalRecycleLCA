namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UnitConversion")]
    public partial class UnitConversion
    {
        public UnitConversion()
        {
            UnitGroups = new HashSet<UnitGroup>();
        }

        public int UnitConversionID { get; set; }

        [StringLength(30)]
        public string Unit { get; set; }

        public int? UnitGroupID { get; set; }

        [StringLength(250)]
        public string LongName { get; set; }

        public double? Conversion { get; set; }

        public virtual UnitGroup UnitGroup { get; set; }

        public virtual ICollection<UnitGroup> UnitGroups { get; set; }
    }
}
