namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ILCDEntity")]
    public partial class ILCDEntity : Entity
    {
        public ILCDEntity()
        {
            Backgrounds = new HashSet<Background>();
            ScenarioBackgrounds = new HashSet<ScenarioBackground>();
            Classifications = new HashSet<Classification>();
            Flows = new HashSet<Flow>();
            FlowProperties = new HashSet<FlowProperty>();
            Fragments = new HashSet<Fragment>();
            LCIAMethods = new HashSet<LCIAMethod>();
            Processes = new HashSet<Process>();
            UnitGroups = new HashSet<UnitGroup>();
        }

        public int ILCDEntityID { get; set; }

        [Required]
        [StringLength(36)]
        [Index("IX_UUID_Version", 1)]
        public string UUID { get; set; }

        [StringLength(15)]
        [Index("IX_UUID_Version", 2, IsUnique = true)]
        public string Version { get; set; }

        public int? DataProviderID { get; set; }

        public int? DataTypeID { get; set; }

        // Inverse navigation
        public virtual ICollection<Background> Backgrounds { get; set; }

        public virtual ICollection<ScenarioBackground> ScenarioBackgrounds { get; set; }

        public virtual ICollection<Classification> Classifications { get; set; }

        public virtual DataProvider DataProvider { get; set; }

        public virtual DataType DataType { get; set; }

        public virtual ICollection<Flow> Flows { get; set; }

        public virtual ICollection<FlowProperty> FlowProperties { get; set; }

        public virtual ICollection<Fragment> Fragments { get; set; }

        public virtual ICollection<LCIAMethod> LCIAMethods { get; set; }

        public virtual ICollection<Process> Processes { get; set; }

        public virtual ICollection<UnitGroup> UnitGroups { get; set; }
    }
}
