namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ParamType")]
    public partial class ParamType
    {
        public ParamType()
        {
            Params = new HashSet<Param>();
        }

        public int ParamTypeID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<Param> Params { get; set; }
    }
}
