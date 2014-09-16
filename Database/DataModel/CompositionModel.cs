namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompositionModel")]
    public partial class CompositionModel
    {
        public CompositionModel()
        {
            CompositionDataSet = new HashSet<CompositionData>();
        }

        [Key]
        public int CompositionModelID { get; set; }

        public int FlowID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<CompositionData> CompositionDataSet { get; set; }

        public virtual Flow Flow { get; set; }
    }
}