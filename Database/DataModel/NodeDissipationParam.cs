namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeDissipationParam")]
    public partial class NodeDissipationParam : Entity
    {
        public int NodeDissipationParamID { get; set; }

        public int? ParamID { get; set; }

        public int? ProcessDissipationID { get; set; }

        public int? FragmentFlowID { get; set; }

        public double? Value { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual Param Param { get; set; }

        public virtual ProcessDissipation ProcessDissipation { get; set; }
    }
}
