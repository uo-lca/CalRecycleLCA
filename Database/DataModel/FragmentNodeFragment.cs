namespace LcaDataModel
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

        public int? FragmentNodeID { get; set; }

        public int? SubFragmentID { get; set; }

        public virtual FragmentNode FragmentNode { get; set; }
    }
}
