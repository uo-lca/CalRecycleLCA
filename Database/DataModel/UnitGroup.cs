namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UnitGroup")]
    public partial class UnitGroup : Entity
    {
        public UnitGroup()
        {
            FlowProperties = new HashSet<FlowProperty>();
            UnitConversions = new HashSet<UnitConversion>();
        }

        public int UnitGroupID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? ReferenceUnitConversionID { get; set; }

        public int? ILCDEntityID { get; set; }

        public virtual ICollection<FlowProperty> FlowProperties { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual ICollection<UnitConversion> UnitConversions { get; set; }

        public virtual UnitConversion UnitConversion { get; set; }
    }
}
