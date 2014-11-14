namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProcessEmissionParam")]
    public partial class ProcessEmissionParam : Entity
    {
        public int ProcessEmissionParamID { get; set; }

        public int? ParamID { get; set; } // should be non-nullable

        public int? ProcessFlowID { get; set; } // should be non-nullable

        public double? Value { get; set; } // should be non-nullable

        public virtual Param Param { get; set; }

        public virtual ProcessFlow ProcessFlow { get; set; }
    }
}
