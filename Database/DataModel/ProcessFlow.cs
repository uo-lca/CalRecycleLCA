namespace LcaDataModel
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcessFlow")]
    public partial class ProcessFlow : Entity
    {
        public ProcessFlow()
        {
            NodeEmissionParams = new HashSet<NodeEmissionParam>();
            ProcessDissipations = new HashSet<ProcessDissipation>();
            ProcessEmissionParams = new HashSet<ProcessEmissionParam>();
        }

        public int ProcessFlowID { get; set; }

        public int? ProcessID { get; set; }

        public int? FlowID { get; set; }

        public int? DirectionID { get; set; }

        [StringLength(15)]
        public string Type { get; set; }

        [StringLength(15)]
        public string VarName { get; set; }

        public double? Magnitude { get; set; }

        public double? Result { get; set; }

        public double? STDev { get; set; }

        [StringLength(15)]
        public string Geography { get; set; }

        public virtual Direction Direction { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ICollection<NodeEmissionParam> NodeEmissionParams { get; set; }

        public virtual Process Process { get; set; }

        public virtual ICollection<ProcessDissipation> ProcessDissipations { get; set; }

        public virtual ICollection<ProcessEmissionParam> ProcessEmissionParams { get; set; }
    }
}
