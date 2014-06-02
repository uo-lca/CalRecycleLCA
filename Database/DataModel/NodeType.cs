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
        public int NodeTypeID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }
    }
}
