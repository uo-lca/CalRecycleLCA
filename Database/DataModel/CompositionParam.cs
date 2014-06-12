namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompositionParam")]
    public partial class CompositionParam
    {
        public int CompositionParamID { get; set; }

        public int? ParamID { get; set; }

        public int? FlowFlowPropertyID { get; set; }

        public virtual FlowFlowProperty FlowFlowProperty { get; set; }

        public virtual Param Param { get; set; }
    }
}
