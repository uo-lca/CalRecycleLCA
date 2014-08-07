namespace LcaDataModel
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
            //turned off these because caused infinite loop and interfere with Json serialization.  Grrrr.
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

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Background> Backgrounds { get; set; }
        public virtual DbSet<BackgroundCache> BackgroundCaches { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategorySystem> CategorySystems { get; set; }
        public virtual DbSet<CharacterizationParam> CharacterizationParams { get; set; }
        public virtual DbSet<Classification> Classifications { get; set; }
        public virtual DbSet<CompositionData> CompositionDatas { get; set; }
        public virtual DbSet<CompositionParam> CompositionParams { get; set; }
        public virtual DbSet<CompostionModel> CompostionModels { get; set; }
        public virtual DbSet<DataProvider> DataProviders { get; set; }
        public virtual DbSet<DataType> DataTypes { get; set; }
        public virtual DbSet<DependencyParam> DependencyParams { get; set; }
        public virtual DbSet<Direction> Directions { get; set; }
        public virtual DbSet<DistributionParam> DistributionParams { get; set; }
        public virtual DbSet<Flow> Flows { get; set; }
        public virtual DbSet<FlowFlowProperty> FlowFlowProperties { get; set; }
        public virtual DbSet<FlowProperty> FlowProperties { get; set; }
        public virtual DbSet<FlowPropertyEmission> FlowPropertyEmissions { get; set; }
        public virtual DbSet<FlowPropertyParam> FlowPropertyParams { get; set; }
        public virtual DbSet<FlowType> FlowTypes { get; set; }
        public virtual DbSet<Fragment> Fragments { get; set; }
        public virtual DbSet<FragmentFlow> FragmentFlows { get; set; }
        public virtual DbSet<FragmentNodeFragment> FragmentNodeFragments { get; set; }
        public virtual DbSet<FragmentNodeProcess> FragmentNodeProcesses { get; set; }
        public virtual DbSet<FragmentStage> FragmentStages { get; set; }
        public virtual DbSet<ILCDEntity> ILCDEntities { get; set; }
        public virtual DbSet<ImpactCategory> ImpactCategories { get; set; }
        public virtual DbSet<IndicatorType> IndicatorTypes { get; set; }
        public virtual DbSet<LCIA> LCIAs { get; set; }
        public virtual DbSet<LCIAMethod> LCIAMethods { get; set; }
        public virtual DbSet<NodeCache> NodeCaches { get; set; }
        public virtual DbSet<NodeDissipationParam> NodeDissipationParams { get; set; }
        public virtual DbSet<NodeEmissionParam> NodeEmissionParams { get; set; }
        public virtual DbSet<NodeType> NodeTypes { get; set; }
        public virtual DbSet<Param> Params { get; set; }
        public virtual DbSet<ParamType> ParamTypes { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<ProcessDissipation> ProcessDissipations { get; set; }
        public virtual DbSet<ProcessDissipationParam> ProcessDissipationParams { get; set; }
        public virtual DbSet<ProcessEmissionParam> ProcessEmissionParams { get; set; }
        public virtual DbSet<ProcessFlow> ProcessFlows { get; set; }
        public virtual DbSet<ProcessType> ProcessTypes { get; set; }
        public virtual DbSet<ReferenceType> ReferenceTypes { get; set; }
        public virtual DbSet<Scenario> Scenarios { get; set; }
        public virtual DbSet<ScenarioBackground> ScenarioBackgrounds { get; set; }
        public virtual DbSet<ScenarioGroup> ScenarioGroups { get; set; }
        public virtual DbSet<ScenarioParam> ScenarioParams { get; set; }
        public virtual DbSet<ScoreCache> ScoreCaches { get; set; }
        public virtual DbSet<UnitConversion> UnitConversions { get; set; }
        public virtual DbSet<UnitGroup> UnitGroups { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Visibility> Visibilities { get; set; }

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
