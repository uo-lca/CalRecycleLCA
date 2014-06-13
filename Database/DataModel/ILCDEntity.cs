namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ILCDEntity")]
    public partial class ILCDEntity
    {
        public ILCDEntity()
        {
            Classifications = new HashSet<Classification>();
            Flows = new HashSet<Flow>();
            FlowProperties = new HashSet<FlowProperty>();
            LCIAMethods = new HashSet<LCIAMethod>();
            Processes = new HashSet<Process>();
            UnitGroups = new HashSet<UnitGroup>();
        }

        [Key]
        [StringLength(36)]
        public string UUID { get; set; }

        [StringLength(15)]
        public string Version { get; set; }

        public int? DataProviderID { get; set; }

        public int? DataTypeID { get; set; }

        public virtual ICollection<Classification> Classifications { get; set; }

        public virtual DataProvider DataProvider { get; set; }

        public virtual DataType DataType { get; set; }

        public virtual ICollection<Flow> Flows { get; set; }

        public virtual ICollection<FlowProperty> FlowProperties { get; set; }

        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }

        public virtual ICollection<Process> Processes { get; set; }

        public virtual ICollection<UnitGroup> UnitGroups { get; set; }
    }
}
