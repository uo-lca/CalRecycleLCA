namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompositionModel")]
    public partial class CompositionModel : Entity
    {
        public CompositionModel()
        {
            CompositionDataSet = new HashSet<CompositionData>();
            CompositionSubstitutions = new HashSet<CompositionSubstitution>();
        }

        [Key]
        public int CompositionModelID { get; set; }

        public int FlowID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<CompositionData> CompositionDataSet { get; set; }

        public virtual Flow Flow { get; set; }

        public virtual ICollection<CompositionSubstitution> CompositionSubstitutions { get; set; }
    }
}
