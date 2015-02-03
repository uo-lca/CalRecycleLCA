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

            FlowPropertyParams = new HashSet<FlowPropertyParam>();

            CompositionParams = new HashSet<CompositionParam>();

            ProcessDissipationParams = new HashSet<ProcessDissipationParam>();

            ProcessEmissionParams = new HashSet<ProcessEmissionParam>();
            
            CharacterizationParams = new HashSet<CharacterizationParam>();

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

        public virtual ICollection<FlowPropertyParam> FlowPropertyParams { get; set; }

        public virtual ICollection<CompositionParam> CompositionParams { get; set; }

        public virtual ICollection<ProcessDissipationParam> ProcessDissipationParams { get; set; }

        public virtual ICollection<ProcessEmissionParam> ProcessEmissionParams { get; set; }

        public virtual ICollection<CharacterizationParam> CharacterizationParams { get; set; }

        //
        // Relationship between Param and *Param should be subclass,
        // and the cardinality should be (0 or 1) to 1. 
        // below are stop-gaps 
        //
        [NotMapped]
        public virtual FlowPropertyParam FlowPropertyParam {
            get { return FlowPropertyParams.FirstOrDefault(); }
        }
        [NotMapped]
        public virtual ProcessEmissionParam ProcessEmissionParam
        {
            get { return ProcessEmissionParams.FirstOrDefault(); }
        }
        [NotMapped]
        public virtual ProcessDissipationParam ProcessDissipationParam
        {
            get { return ProcessDissipationParams.FirstOrDefault(); }
        }
        [NotMapped]
        public virtual CharacterizationParam CharacterizationParam
        {
            get { return CharacterizationParams.FirstOrDefault(); }
        }
    }
}
