namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FlowPropertyParam")]
    public partial class FlowPropertyParam : Entity
    {
        public int FlowPropertyParamID { get; set; }

        public int ParamID { get; set; }

        public int FlowFlowPropertyID { get; set; }

        public double Value { get; set; }

        public virtual FlowFlowProperty FlowFlowProperty { get; set; }

        public virtual Param Param { get; set; }
    }
}
