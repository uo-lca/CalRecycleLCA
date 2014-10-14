namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReferenceType")]
    public partial class ReferenceType : Entity
    {
        public ReferenceType()
        {
            Processes = new HashSet<Process>();
        }

        public int ReferenceTypeID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<Process> Processes { get; set; }
    }
}
