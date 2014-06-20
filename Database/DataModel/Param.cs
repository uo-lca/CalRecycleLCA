namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Param")]
    public partial class Param
    {
        public Param()
        {
            CharacterizationParams = new HashSet<CharacterizationParam>();
            CompositionParams = new HashSet<CompositionParam>();
            DependencyParams = new HashSet<DependencyParam>();
            NodeEmissionParams = new HashSet<NodeEmissionParam>();
            ProcessDissipationParams = new HashSet<ProcessDissipationParam>();
            ProcessEmissionParams = new HashSet<ProcessEmissionParam>();
        }

        public int ParamID { get; set; }

        public int? ParamTypeID { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        public double? Min { get; set; }

        public double? Typ { get; set; }

        public double? Max { get; set; }

        public virtual ICollection<CharacterizationParam> CharacterizationParams { get; set; }

        public virtual ICollection<CompositionParam> CompositionParams { get; set; }

        public virtual ICollection<DependencyParam> DependencyParams { get; set; }

        public virtual ICollection<NodeEmissionParam> NodeEmissionParams { get; set; }

        public virtual ParamType ParamType { get; set; }

        public virtual ICollection<ProcessDissipationParam> ProcessDissipationParams { get; set; }

        public virtual ICollection<ProcessEmissionParam> ProcessEmissionParams { get; set; }

        public virtual ScenarioParam ScenarioParam { get; set; }
    }
}