namespace LcaDataModel
{
    using Repository;
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcessType")]
    public partial class ProcessType : Entity
    {
        public ProcessType()
        {
            Processes = new HashSet<Process>();
        }

        public int ProcessTypeID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<Process> Processes { get; set; }
    }
}
