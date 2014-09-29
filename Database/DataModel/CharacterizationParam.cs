namespace LcaDataModel
{
    using Repository;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CharacterizationParam")]
    public partial class CharacterizationParam : Entity
    {
        public int CharacterizationParamID { get; set; }

        public int? ParamID { get; set; }

        public int? LCAID { get; set; }

        public double? Value { get; set; }

        public virtual LCIA LCIA { get; set; }

        public virtual Param Param { get; set; }
    }
}
