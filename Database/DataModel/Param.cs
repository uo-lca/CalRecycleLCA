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

        public virtual CharacterizationParam CharacterizationParam { get; set; }
    }
}
