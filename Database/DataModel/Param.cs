namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("Param")]
    public partial class Param : Entity
    {
        public Param()
        {
            DependencyParams = new HashSet<DependencyParam>();

            CharacterizationParams = new HashSet<CharacterizationParam>();

            CompositionParams = new HashSet<CompositionParam>();
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ParamID { get; set; }

        public int? ParamTypeID { get; set; }

        public int ScenarioID { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public double? Min { get; set; }

        public double? Typ { get; set; }

        public double? Max { get; set; }

        public virtual ParamType ParamType { get; set; }
        public virtual Scenario Scenario { get; set; }
        
        public virtual ICollection<DependencyParam> DependencyParams { get; set; }

        public virtual FlowPropertyParam FlowPropertyParam { get; set; }

        public virtual ICollection<CompositionParam> CompositionParams { get; set; }

        public virtual ProcessDissipationParam ProcessDissipationParam { get; set; }

        public virtual ProcessEmissionParam ProcessEmissionParam { get; set; }

        //
        // Relationship between Param and CharacterizationParam should be subclass,
        // and the cardinality should be (0 or 1) to 1. 
        // The following is a stop-gap until model is revised.
        //
        public virtual ICollection<CharacterizationParam> CharacterizationParams { get; set; }
        [NotMapped]
        public virtual CharacterizationParam CharacterizationParam {
            get { return CharacterizationParams.FirstOrDefault(); }
        }
    }
}
