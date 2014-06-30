namespace Data.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcessEmissionParam")]
    public partial class ProcessEmissionParam
    {
        public int ProcessEmissionParamID { get; set; }

        public int? ParamID { get; set; }

        public int? ProcessFlowID { get; set; }

        public virtual Param Param { get; set; }

        public virtual ProcessFlow ProcessFlow { get; set; }
    }
}
