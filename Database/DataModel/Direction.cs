namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Direction")]
    public partial class Direction
    {
        public Direction()
        {
            FragmentEdges = new HashSet<FragmentEdge>();
            LCIAs = new HashSet<LCIA>();
            NodeEmissions = new HashSet<NodeEmission>();
            ProcessFlows = new HashSet<ProcessFlow>();
        }

        public int DirectionID { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<FragmentEdge> FragmentEdges { get; set; }

        public virtual ICollection<LCIA> LCIAs { get; set; }

        public virtual ICollection<NodeEmission> NodeEmissions { get; set; }

        public virtual ICollection<ProcessFlow> ProcessFlows { get; set; }
    }
}
