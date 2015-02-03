namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LCIAMethod")]
    public partial class LCIAMethod : Entity
    {
        public LCIAMethod()
        {
            BackgroundCaches = new HashSet<BackgroundCache>();
            LCIAs = new HashSet<LCIA>();
            ScoreCaches = new HashSet<ScoreCache>();
        }

        public int LCIAMethodID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(60)]
        public string Methodology { get; set; }

        public int ImpactCategoryID { get; set; }

        public string ImpactIndicator { get; set; }

        [StringLength(60)]
        public string ReferenceYear { get; set; }

        [StringLength(255)]
        public string Duration { get; set; }

        [StringLength(60)]
        public string ImpactLocation { get; set; }

        public int? IndicatorTypeID { get; set; }

        public bool? Normalization { get; set; }

        public bool? Weighting { get; set; }

        public string UseAdvice { get; set; }

        public int ReferenceFlowPropertyID { get; set; }

        public int ILCDEntityID { get; set; }

        public virtual ICollection<BackgroundCache> BackgroundCaches { get; set; }

        public virtual FlowProperty FlowProperty { get; set; }

        public virtual ILCDEntity ILCDEntity { get; set; }

        public virtual ImpactCategory ImpactCategory { get; set; }

        public virtual IndicatorType IndicatorType { get; set; }

        public virtual ICollection<LCIA> LCIAs { get; set; }

        public virtual ICollection<ScoreCache> ScoreCaches { get; set; }
    }
}
