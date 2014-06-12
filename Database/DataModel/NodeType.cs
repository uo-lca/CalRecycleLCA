namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeType")]
    public partial class NodeType
    {
        public NodeType()
        {
            Backgrounds = new HashSet<Background>();
            FragmentNodes = new HashSet<FragmentNode>();
            ScenarioBackgrounds = new HashSet<ScenarioBackground>();
        }

        public int NodeTypeID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<Background> Backgrounds { get; set; }

        public virtual ICollection<FragmentNode> FragmentNodes { get; set; }

        public virtual ICollection<ScenarioBackground> ScenarioBackgrounds { get; set; }
    }
}
