namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentNodeProcess")]
    public partial class FragmentNodeProcess
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentNodeProcessID { get; set; }

        public int? FragmentFlowID { get; set; }

        public int? ProcessID { get; set; }

        public int? FlowID { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual Process Process { get; set; }
    }
}
