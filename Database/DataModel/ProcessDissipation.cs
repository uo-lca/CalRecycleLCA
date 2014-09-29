namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcessDissipation")]
    public partial class ProcessDissipation : Entity
    {
        public ProcessDissipation()
        {
            NodeDissipationParams = new HashSet<NodeDissipationParam>();
            ProcessDissipationParams = new HashSet<ProcessDissipationParam>();
        }

        public int ProcessDissipationID { get; set; }

        public int? ProcessFlowID { get; set; }

        public double? EmissionFactor { get; set; }

        public virtual ICollection<NodeDissipationParam> NodeDissipationParams { get; set; }

        public virtual ProcessFlow ProcessFlow { get; set; }

        public virtual ICollection<ProcessDissipationParam> ProcessDissipationParams { get; set; }
    }
}
