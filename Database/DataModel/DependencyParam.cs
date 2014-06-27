namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DependencyParam")]
    public partial class DependencyParam
    {
        public DependencyParam()
        {
            DistributionParams = new HashSet<DistributionParam>();
            DistributionParams1 = new HashSet<DistributionParam>();
        }

        public int DependencyParamID { get; set; }

        public int? ParamID { get; set; }

        public int? FragmentFlowID { get; set; }

        public double? Value { get; set; }

        public virtual FragmentFlow FragmentFlow { get; set; }

        public virtual Param Param { get; set; }

        public virtual ICollection<DistributionParam> DistributionParams { get; set; }

        public virtual ICollection<DistributionParam> DistributionParams1 { get; set; }
    }
}
