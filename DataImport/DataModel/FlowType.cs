namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowType")]
    public partial class FlowType
    {
        public FlowType()
        {
            Flows = new HashSet<Flow>();
        }

        public int FlowTypeID { get; set; }

        [StringLength(100)]
        public string Type { get; set; }

        public virtual ICollection<Flow> Flows { get; set; }
    }
}
