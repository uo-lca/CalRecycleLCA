namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcessDissipationParam")]
    public partial class ProcessDissipationParam
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProcessDissipationParamID { get; set; }

        public int? ParamID { get; set; }

        public int? ProcessDissipationID { get; set; }

        public virtual Param Param { get; set; }

        public virtual ProcessDissipation ProcessDissipation { get; set; }
    }
}
