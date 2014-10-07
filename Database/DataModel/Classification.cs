namespace LcaDataModel
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Classification")]
    public partial class Classification : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClassificationID { get; set; }

        public int? CategoryID { get; set; }

        public int? ILCDEntityID { get; set; }

        public virtual Category Category { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }
    }
}
