namespace LcaDataModel {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EntityDataModel : DbContext {
        public EntityDataModel()
            : base("name=EntityDataModel") {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategorySystem> CategorySystems { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Classification> Classifications { get; set; }
        public virtual DbSet<DataProvider> DataProviders { get; set; }
        public virtual DbSet<DataType> DataTypes { get; set; }
        public virtual DbSet<Direction> Directions { get; set; }
        public virtual DbSet<Flow> Flows { get; set; }
        public virtual DbSet<FlowFlowProperty> FlowFlowProperties { get; set; }
        public virtual DbSet<FlowProperty> FlowProperties { get; set; }
        public virtual DbSet<FlowType> FlowTypes { get; set; }
        public virtual DbSet<Fragment> Fragments { get; set; }
        public virtual DbSet<FragmentEdge> FragmentEdges { get; set; }
        public virtual DbSet<FragmentEmission> FragmentEmissions { get; set; }
        public virtual DbSet<FragmentScore> FragmentScores { get; set; }
        public virtual DbSet<FragmentStage> FragmentStages { get; set; }
        public virtual DbSet<ImpactCategory> ImpactCategories { get; set; }
        public virtual DbSet<IndicatorType> IndicatorTypes { get; set; }
        public virtual DbSet<LCIA> LCIAs { get; set; }
        public virtual DbSet<LCIAMethod> LCIAMethods { get; set; }
        public virtual DbSet<NodeType> NodeTypes { get; set; }
        public virtual DbSet<Param> Params { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<ProcessFlow> ProcessFlows { get; set; }
        public virtual DbSet<ScenarioSet> ScenarioSets { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UnitConversion> UnitConversions { get; set; }
        public virtual DbSet<UnitGroup> UnitGroups { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<FragmentNode> FragmentNodes { get; set; }
        public virtual DbSet<ParamType> ParamTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Category>()
                .Property(e => e.Hier)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.ClassID_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.Parent_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.ClassName_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<CategorySystem>()
                .Property(e => e.CategorySystem1)
                .IsUnicode(false);

            modelBuilder.Entity<CategorySystem>()
                .Property(e => e.URI)
                .IsUnicode(false);

            modelBuilder.Entity<CategorySystem>()
                .Property(e => e.Delimeter)
                .IsUnicode(false);

            modelBuilder.Entity<CategorySystem>()
                .Property(e => e.DataType_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.ExternalClassID)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Categories)
                .WithOptional(e => e.Class)
                .HasForeignKey(e => e.ClassID);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Categories1)
                .WithOptional(e => e.Class1)
                .HasForeignKey(e => e.ParentClassID);

            modelBuilder.Entity<Classification>()
                .Property(e => e.ClassificationUUID)
                .IsUnicode(false);

            modelBuilder.Entity<Classification>()
                .Property(e => e.ClassID_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<Classification>()
                .Property(e => e.CategorySystem_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<DataProvider>()
                .Property(e => e.DataProviderUUID)
                .IsUnicode(false);

            modelBuilder.Entity<DataProvider>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<DataType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Direction>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .Property(e => e.FlowUUID)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .Property(e => e.FlowVersion)
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .Property(e => e.CASNumber)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .Property(e => e.FlowType_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .Property(e => e.ReferenceFlowProperty_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<FlowFlowProperty>()
                .Property(e => e.FlowPropertyVersionUUID)
                .IsUnicode(false);

            modelBuilder.Entity<FlowFlowProperty>()
                .Property(e => e.FlowProperty_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<FlowFlowProperty>()
                .Property(e => e.FlowReference_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProperty>()
                .Property(e => e.FlowPropertyUUID)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<FlowProperty>()
                .Property(e => e.FlowPropertyVersion)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProperty>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProperty>()
                .Property(e => e.UnitGroup_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<FlowType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Fragment>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FragmentStage>()
                .Property(e => e.StageName)
                .IsUnicode(false);

            modelBuilder.Entity<ImpactCategory>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<IndicatorType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<LCIA>()
                .Property(e => e.LCIAUUID)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<LCIA>()
                .Property(e => e.Location)
                .IsUnicode(false);

            modelBuilder.Entity<LCIA>()
                .Property(e => e.Flow_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<LCIA>()
                .Property(e => e.Direction_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.LCIAMethodUUID)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.LCIAMethodVersion)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.Methodology)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.ImpactIndicator)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.ReferenceYear)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.Duration)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.ImpactLocation)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.UseAdvice)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.Source)
                .IsUnicode(false);

            modelBuilder.Entity<LCIAMethod>()
                .Property(e => e.ReferenceQuantity)
                .IsUnicode(false);

            modelBuilder.Entity<NodeType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Param>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.ProcessUUID)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.ProcessVersion)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.Year)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.Geography)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.ReferenceFlow_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.RefererenceType)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.ProcessType)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.Diagram)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.ProcessUUID)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.VarName)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.Flow_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.Direction_SQL)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.Geography)
                .IsUnicode(false);

            modelBuilder.Entity<ScenarioSet>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Source>()
                .Property(e => e.SourceUUID)
                .IsUnicode(false);

            modelBuilder.Entity<Source>()
                .Property(e => e.SourceVersion)
                .IsUnicode(false);

            modelBuilder.Entity<Source>()
                .Property(e => e.Source1)
                .IsUnicode(false);

            modelBuilder.Entity<Source>()
                .Property(e => e.Citation)
                .IsUnicode(false);

            modelBuilder.Entity<Source>()
                .Property(e => e.PubType)
                .IsUnicode(false);

            modelBuilder.Entity<Source>()
                .Property(e => e.URI)
                .IsUnicode(false);

            modelBuilder.Entity<UnitConversion>()
                .Property(e => e.UnitConversionUUID)
                .IsUnicode(false);

            modelBuilder.Entity<UnitConversion>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<UnitGroup>()
                .Property(e => e.UnitGroupUUID)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<UnitGroup>()
                .Property(e => e.Version)
                .IsUnicode(false);

            modelBuilder.Entity<UnitGroup>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UnitGroup>()
                .Property(e => e.ReferenceUnit)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FragmentNode>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ParamType>()
                .Property(e => e.Name)
                .IsUnicode(false);
        }
    }
}
