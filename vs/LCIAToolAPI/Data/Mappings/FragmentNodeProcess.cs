namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentNodeProcess")]
    public partial class FragmentNodeProcess
    {
        public int FragmentNodeProcessID { get; set; }

        public int? FragmentFlowID { get; set; }

        public int? ProcessID { get; set; }
    }
}
