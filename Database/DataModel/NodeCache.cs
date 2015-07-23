namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeCache")]
    public partial class NodeCache : Entity
    {

        //public int NodeCacheID { get; set; }

        [Key, Column(Order=1)]
        public int FragmentFlowID { get; set; }

        [Key, Column(Order = 2)]
        public int ScenarioID { get; set; } 

        public double NodeWeight { get; set; }

        public double FlowMagnitude { get; set; }

        public int? ILCDEntityID { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual Scenario Scenario { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }
    }
}
