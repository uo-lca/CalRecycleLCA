namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UnitGroup")]
    public partial class UnitGroup
    {
        public UnitGroup()
        {
            FlowProperties = new HashSet<FlowProperty>();
            UnitConversions = new HashSet<UnitConversion>();
        }

        public int UnitGroupID { get; set; }

        [Required]
        [StringLength(36)]
        public string UnitGroupUUID { get; set; }

        [StringLength(15)]
        public string Version { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string ReferenceUnit { get; set; }

        

        public int CreatedBy { get; set; }

        

        public int UpdatedBy { get; set; }

        public bool Voided { get; set; }

        public virtual ICollection<FlowProperty> FlowProperties { get; set; }

        public virtual ICollection<UnitConversion> UnitConversions { get; set; }
    }
}
