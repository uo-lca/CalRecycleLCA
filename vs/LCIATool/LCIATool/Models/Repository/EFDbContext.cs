using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LCIATool.Models.Repository
{
    public class EFDbContext : DbContext
    {
        public DbSet<LCIA> LCIAs { get; set; }
        public DbSet<LCIAMethod> LCIAMethods { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<ProcessFlow> ProcessFlows { get; set; }
    }
}