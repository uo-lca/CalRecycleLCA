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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FragmentNodeFragmentID { get; set; }

        public int? FragmentFlowID { get; set; }

        public int? SubFragmentID { get; set; }

        public int? FlowID { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual Fragment Fragment { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }
    }
}
