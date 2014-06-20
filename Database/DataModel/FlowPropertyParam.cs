namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowPropertyParam")]
    public partial class FlowPropertyParam
    {
        public int FlowPropertyParamID { get; set; }

        public int? ParamID { get; set; }

        public int? FlowFlowPropertyID { get; set; }

        public virtual FlowFlowProperty FlowFlowProperty { get; set; }

        public virtual FlowPropertyParam FlowPropertyParam1 { get; set; }

        public virtual FlowPropertyParam FlowPropertyParam2 { get; set; }
    }
}