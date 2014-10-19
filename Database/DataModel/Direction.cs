namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Direction")]
    public partial class Direction : Entity
    {
        public Direction()
        {
            Backgrounds = new HashSet<Background>();
            Scenarios = new HashSet<Scenario>();
            ScenarioBackgrounds = new HashSet<ScenarioBackground>();
            FragmentFlows = new HashSet<FragmentFlow>();
            LCIAs = new HashSet<LCIA>();
            ProcessFlows = new HashSet<ProcessFlow>();
        }

        public int DirectionID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Background> Backgrounds { get; set; }

        public virtual ICollection<Scenario> Scenarios { get; set; }

        public virtual ICollection<ScenarioBackground> ScenarioBackgrounds { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<LCIA> LCIAs { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
