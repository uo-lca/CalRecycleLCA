namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompostionModel")]
    public partial class CompostionModel
    {
        public CompostionModel()
        {
            CompositionDatas = new HashSet<CompositionData>();
        }

        [Key]
        public int CompositionModelID { get; set; }

        public int? FlowID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<CompositionData> CompositionDatas { get; set; }

        public virtual Flow Flow { get; set; }
    }
}
