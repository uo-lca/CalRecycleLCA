namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Param")]
    public partial class Param : Entity
    {
        public Param()
        {
            DependencyParams = new HashSet<DependencyParam>();
            FlowPropertyParams = new HashSet<FlowPropertyParam>();

            CompositionParams = new HashSet<CompositionParam>();
            ProcessDissipationParams = new HashSet<ProcessDissipationParam>();
            NodeDissipationParams = new HashSet<NodeDissipationParam>();
            ProcessEmissionParams = new HashSet<ProcessEmissionParam>();
            NodeEmissionParams = new HashSet<NodeEmissionParam>();
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

        public virtual ICollection<NodeDissipationParam> NodeDissipationParams { get; set; }

        public virtual ICollection<ProcessEmissionParam> ProcessEmissionParams { get; set; }

        public virtual ICollection<NodeEmissionParam> NodeEmissionParams { get; set; }

        public virtual ICollection<CharacterizationParam> CharacterizationParams { get; set; }
    }
}
