namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowType")]
    public partial class FlowType : Entity
    {
        public FlowType()
        {
            Flows = new HashSet<Flow>();
        }

        public int FlowTypeID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Flow> Flows { get; set; }
    }
}
