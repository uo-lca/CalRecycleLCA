namespace Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.Validation;

    public partial class UsedOilLCAContext : DbContext, IDbContext
    {
              static UsedOilLCAContext()
    {
        Database.SetInitializer<UsedOilLCAContext>(null);
    }

        public UsedOilLCAContext()
            : base("name=UsedOilLCAContext")
        {
            //turned off the because caused infinite loop.  Grrrr.
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public override int SaveChanges()
        {
            try
            {
                //this.ApplyStateChanges();
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        public DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public DbSet<Background> Backgrounds { get; set; }
        public DbSet<BackgroundCache> BackgroundCaches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategorySystem> CategorySystems { get; set; }
        public DbSet<CharacterizationParam> CharacterizationParams { get; set; }
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<CompositionData> CompositionDatas { get; set; }
        public DbSet<CompositionParam> CompositionParams { get; set; }
        public DbSet<CompostionModel> CompostionModels { get; set; }
        public DbSet<DataProvider> DataProviders { get; set; }
        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<DependencyParam> DependencyParams { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<DistributionParam> DistributionParams { get; set; }
        public DbSet<Flow> Flows { get; set; }
        public DbSet<FlowFlowProperty> FlowFlowProperties { get; set; }
        public DbSet<FlowProperty> FlowProperties { get; set; }
        public DbSet<FlowPropertyEmission> FlowPropertyEmissions { get; set; }
        public DbSet<FlowPropertyParam> FlowPropertyParams { get; set; }
        public DbSet<FlowType> FlowTypes { get; set; }
        public DbSet<Fragment> Fragments { get; set; }
        public DbSet<FragmentFlow> FragmentFlows { get; set; }
        public DbSet<FragmentNodeFragment> FragmentNodeFragments { get; set; }
        public DbSet<FragmentNodeProcess> FragmentNodeProcesses { get; set; }
        public DbSet<FragmentStage> FragmentStages { get; set; }
        public DbSet<ILCDEntity> ILCDEntities { get; set; }
        public DbSet<ImpactCategory> ImpactCategories { get; set; }
        public DbSet<IndicatorType> IndicatorTypes { get; set; }
        public DbSet<LCIA> LCIAs { get; set; }
        public DbSet<LCIAMethod> LCIAMethods { get; set; }
        public DbSet<NodeCache> NodeCaches { get; set; }
        public DbSet<NodeDissipationParam> NodeDissipationParams { get; set; }
        public DbSet<NodeEmissionParam> NodeEmissionParams { get; set; }
        public DbSet<NodeType> NodeTypes { get; set; }
        public DbSet<Param> Params { get; set; }
        public DbSet<ParamType> ParamTypes { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<ProcessDissipation> ProcessDissipations { get; set; }
        public DbSet<ProcessDissipationParam> ProcessDissipationParams { get; set; }
        public DbSet<ProcessEmissionParam> ProcessEmissionParams { get; set; }
        public DbSet<ProcessFlow> ProcessFlows { get; set; }
        public DbSet<ProcessType> ProcessTypes { get; set; }
        public DbSet<ReferenceType> ReferenceTypes { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<ScenarioBackground> ScenarioBackgrounds { get; set; }
        public DbSet<ScenarioGroup> ScenarioGroups { get; set; }
        public DbSet<ScenarioParam> ScenarioParams { get; set; }
        public DbSet<ScoreCache> ScoreCaches { get; set; }
        public DbSet<UnitConversion> UnitConversions { get; set; }
        public DbSet<UnitGroup> UnitGroups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Visibility> Visibilities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

           

            modelBuilder.Entity<Category>()
                .Property(e => e.ExternalClassID)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.Category1)
                .WithOptional(e => e.Category2)
                .HasForeignKey(e => e.ParentCategoryID);

            modelBuilder.Entity<CategorySystem>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<CategorySystem>()
                .Property(e => e.Delimiter)
                .IsUnicode(false);

            modelBuilder.Entity<CompostionModel>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<DataProvider>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<DataProvider>()
                .Property(e => e.DirName)
                .IsUnicode(false);

            modelBuilder.Entity<DataType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<DependencyParam>()
                .HasMany(e => e.DistributionParams)
                .WithOptional(e => e.DependencyParam)
                .HasForeignKey(e => e.ConservationParamID);

            modelBuilder.Entity<DependencyParam>()
                .HasMany(e => e.DistributionParams1)
                .WithOptional(e => e.DependencyParam1)
                .HasForeignKey(e => e.DependencyParamID);

            modelBuilder.Entity<Direction>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .Property(e => e.CASNumber)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Flow>()
                .HasMany(e => e.Processes)
                .WithOptional(e => e.Flow)
                .HasForeignKey(e => e.ReferenceFlowID);

            modelBuilder.Entity<FlowProperty>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FlowProperty>()
                .HasMany(e => e.Flows)
                .WithOptional(e => e.FlowProperty)
                .HasForeignKey(e => e.ReferenceFlowProperty);

            modelBuilder.Entity<FlowProperty>()
                .HasMany(e => e.FragmentFlows)
                .WithOptional(e => e.FlowProperty)
                .HasForeignKey(e => e.ReferenceFlowPropertyID);

            modelBuilder.Entity<FlowProperty>()
                .HasMany(e => e.LCIAMethods)
                .WithOptional(e => e.FlowProperty)
                .HasForeignKey(e => e.ReferenceQuantity);

            modelBuilder.Entity<FlowType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Fragment>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Fragment>()
                .HasMany(e => e.FragmentFlows)
                .WithOptional(e => e.Fragment)
                .HasForeignKey(e => e.FragmentID);

            modelBuilder.Entity<Fragment>()
                .HasMany(e => e.FragmentNodeFragments)
                .WithOptional(e => e.Fragment)
                .HasForeignKey(e => e.SubFragmentID);

            modelBuilder.Entity<FragmentFlow>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FragmentFlow>()
                .HasMany(e => e.Fragments)
                .WithOptional(e => e.FragmentFlow)
                .HasForeignKey(e => e.ReferenceFragmentFlowID);

            modelBuilder.Entity<FragmentFlow>()
                .HasMany(e => e.FragmentFlow1)
                .WithOptional(e => e.FragmentFlow2)
                .HasForeignKey(e => e.ParentFragmentFlowID);

            modelBuilder.Entity<FragmentStage>()
                .Property(e => e.StageName)
                .IsUnicode(false);

            modelBuilder.Entity<ILCDEntity>()
                .Property(e => e.UUID)
                .IsUnicode(false);

            modelBuilder.Entity<ILCDEntity>()
                .Property(e => e.Version)
                .IsUnicode(false);

            modelBuilder.Entity<ImpactCategory>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<IndicatorType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<LCIA>()
                .Property(e => e.Geography)
                .IsUnicode(false);

            modelBuilder.Entity<LCIA>()
                .HasMany(e => e.CharacterizationParams)
                .WithOptional(e => e.LCIA)
                .HasForeignKey(e => e.LCAID);

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

            modelBuilder.Entity<NodeType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Param>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ParamType>()
                .Property(e => e.Name)
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

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.VarName)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.Geography)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ReferenceType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Scenario>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ScenarioGroup>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UnitConversion>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<UnitConversion>()
                .Property(e => e.LongName)
                .IsUnicode(false);

            modelBuilder.Entity<UnitConversion>()
                .HasMany(e => e.UnitGroups)
                .WithOptional(e => e.UnitConversion)
                .HasForeignKey(e => e.ReferenceUnitConversionID);

            modelBuilder.Entity<UnitGroup>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UnitGroup>()
                .HasMany(e => e.UnitConversions)
                .WithOptional(e => e.UnitGroup)
                .HasForeignKey(e => e.UnitGroupID);

            modelBuilder.Entity<User>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ScenarioGroups)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.OwnedBy);

            modelBuilder.Entity<Visibility>()
                .Property(e => e.Name)
                .IsUnicode(false);
        }
    }
}
