namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompositionParam")]
    public partial class CompositionParam : Entity
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CompositionParamID { get; set; }

        public int ParamID { get; set; }

        public int CompositionDataID { get; set; }

        public double Value { get; set; }

        public virtual CompositionData CompositionData { get; set; }

        public virtual Param Param { get; set; }

    }
}
