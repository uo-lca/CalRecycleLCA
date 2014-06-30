namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeEmissionParam")]
    public partial class NodeEmissionParam
    {
        public int NodeEmissionParamID { get; set; }

        public int? ParamID { get; set; }

        public int? ProcessFlowID { get; set; }

        public int? FragmentFlowID { get; set; }

        public double? Value { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual Param Param { get; set; }

        public virtual ProcessFlow ProcessFlow { get; set; }
    }
}
