﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ILCD_matlab_Entities : DbContext
    {
        public ILCD_matlab_Entities()
            : base("name=ILCD_matlab_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LCIA_DUPE> LCIA_DUPE { get; set; }
        public virtual DbSet<LCIAMethod_DUPE> LCIAMethod_DUPE { get; set; }
    }
}