namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ConservationParam")]
    public partial class ConservationParam : Entity
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DependencyParamID { get; set; } // to balancing Param

        public int FragmentFlowID { get; set; } // of parent node with conservation applied

        public int FlowPropertyID { get; set; } // property that is conserved

        public int DirectionID { get; set; } // direction of balancing Param w.r.t. parent node

        public virtual DependencyParam DependencyParam { get; set; }
        public virtual FragmentFlow FragmentFlow { get; set; }
        public virtual FlowProperty FlowProperty { get; set; }
    }
}
