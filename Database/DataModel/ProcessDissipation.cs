namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
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
            ProcessDissipationParams = new HashSet<ProcessDissipationParam>();
        }

        public int ProcessDissipationID { get; set; }

        public int ProcessID { get; set; }

        public int FlowPropertyEmissionID { get; set; }

        public double? EmissionFactor { get; set; }

        public virtual Process Process { get; set; }

        public virtual FlowPropertyEmission FlowPropertyEmission { get; set; }

        public virtual ICollection<ProcessDissipationParam> ProcessDissipationParams { get; set; }
    }
}
