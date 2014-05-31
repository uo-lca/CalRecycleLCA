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
        public int UnitConversionID { get; set; }

        [StringLength(36)]
        public string UnitConversionUUID { get; set; }

        [StringLength(30)]
        public string Unit { get; set; }

        public int? UnitGroupID { get; set; }

        public double? Conversion { get; set; }

        [Column("Ind-sql")]
        public int? Ind_sql { get; set; }

        

        public int CreatedBy { get; set; }

        

        public int UpdatedBy { get; set; }

        public bool Voided { get; set; }

        public virtual UnitGroup UnitGroup { get; set; }
    }
}
