using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using IlcdDataLoader.Models.Mapping;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class LCAToolDbContext : DbContext
    {
        static LCAToolDbContext()
        {
            Database.SetInitializer<LCAToolDbContext>(null);
        }

        public LCAToolDbContext()
            : base("Name=LCAToolDbContext")
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategorySystem> CategorySystems { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<DataProvider> DataProviders { get; set; }
        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Flow> Flows { get; set; }
        public DbSet<FlowFlowProperty> FlowFlowProperties { get; set; }
        public DbSet<FlowProperty> FlowProperties { get; set; }
        public DbSet<FlowType> FlowTypes { get; set; }
        public DbSet<ImpactCategory> ImpactCategories { get; set; }
        public DbSet<IndicatorType> IndicatorTypes { get; set; }
        public DbSet<LCIA> LCIAs { get; set; }
        public DbSet<LCIAMethod> LCIAMethods { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<ProcessFlow> ProcessFlows { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<UnitConversion> UnitConversions { get; set; }
        public DbSet<UnitGroup> UnitGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CategorySystemMap());
            modelBuilder.Configurations.Add(new ClassMap());
            modelBuilder.Configurations.Add(new ClassificationMap());
            modelBuilder.Configurations.Add(new DataProviderMap());
            modelBuilder.Configurations.Add(new DataTypeMap());
            modelBuilder.Configurations.Add(new DirectionMap());
            modelBuilder.Configurations.Add(new FlowMap());
            modelBuilder.Configurations.Add(new FlowFlowPropertyMap());
            modelBuilder.Configurations.Add(new FlowPropertyMap());
            modelBuilder.Configurations.Add(new FlowTypeMap());
            modelBuilder.Configurations.Add(new ImpactCategoryMap());
            modelBuilder.Configurations.Add(new IndicatorTypeMap());
            modelBuilder.Configurations.Add(new LCIAMap());
            modelBuilder.Configurations.Add(new LCIAMethodMap());
            modelBuilder.Configurations.Add(new ProcessMap());
            modelBuilder.Configurations.Add(new ProcessFlowMap());
            modelBuilder.Configurations.Add(new SourceMap());
            modelBuilder.Configurations.Add(new UnitConversionMap());
            modelBuilder.Configurations.Add(new UnitGroupMap());
        }

        public Dictionary<string, int> UnitGroupIDs { get; set; }
    }
}
