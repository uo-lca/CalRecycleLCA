//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataImport
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    
    public partial class LCIA_DUPE
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Ind { get; set; }
        public string UUID { get; set; }
        public string Flow { get; set; }
        public string Direction { get; set; }
        public float Factor { get; set; }
    
        public virtual LCIAMethod_DUPE LCIAMethod_DUPE { get; set; }
    }
}
