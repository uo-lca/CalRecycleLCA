namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FragmentStage")]
    public partial class FragmentStage
    {
        public int FragmentStageID { get; set; }

        public int? FragmentID { get; set; }

        [StringLength(255)]
        public string StageName { get; set; }
    }
}
