namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentNodeFragment")]
    public partial class FragmentNodeFragment
    {
        public int FragmentNodeFragmentID { get; set; }

        public int? FragmentFlowID { get; set; }

        public int? SubFragmentID { get; set; }
    }
}
