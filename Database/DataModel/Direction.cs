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
            BackgroundSubstitutions = new HashSet<BackgroundSubstitution>();
            FragmentFlows = new HashSet<FragmentFlow>();
            LCIAs = new HashSet<LCIA>();
            ProcessFlows = new HashSet<ProcessFlow>();
        }

        public static int comp(int directionId)
        {
            if (directionId == 1)
                return 2;
            else
                return 1;
        }

        public int DirectionID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Background> Backgrounds { get; set; }

        public virtual ICollection<Scenario> Scenarios { get; set; }

        public virtual ICollection<BackgroundSubstitution> BackgroundSubstitutions { get; set; }

        public virtual ICollection<FragmentFlow> FragmentFlows { get; set; }

        public virtual ICollection<LCIA> LCIAs { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
