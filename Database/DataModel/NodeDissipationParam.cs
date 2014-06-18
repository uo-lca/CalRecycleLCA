namespace LcaDataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NodeDissipationParam")]
    public partial class NodeDissipationParam
    {
        public int NodeDissipationParamID { get; set; }

        public int? ParamID { get; set; }

        public int? NodeDissipationID { get; set; }
    }
}
