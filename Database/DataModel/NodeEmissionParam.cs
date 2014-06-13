namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeEmissionParam")]
    public partial class NodeEmissionParam
    {
        public int NodeEmissionParamID { get; set; }

        public int? ParamID { get; set; }

        public int? NodeEmissionID { get; set; }

        public virtual NodeEmission NodeEmission { get; set; }

        public virtual Param Param { get; set; }
    }
}
