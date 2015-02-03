namespace LcaDataModel
{
    using Repository.Pattern.Ef6;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CharacterizationParam")]
    public partial class CharacterizationParam : Entity
    {
        public int CharacterizationParamID { get; set; }

        public int ParamID { get; set; }

        public int LCIAID { get; set; }

        public double Value { get; set; }

        public virtual LCIA LCIA { get; set; }

        public virtual Param Param { get; set; }
    }
}
