namespace LcaDataModel {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using Repository.Pattern.Ef6;

    public partial class EntityDataModel : DataContext {
        public EntityDataModel()
            : base("EntityDataModel") {
                SyncObjectStateEnabled = false;
        }

        public EntityDataModel(String connName)
            : base(connName) {
        }

        public virtual DbSet<Background> Backgrounds { get; set; }
        public virtual DbSet<BackgroundCache> BackgroundCaches { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategorySystem> CategorySystems { get; set; }
        public virtual DbSet<CharacterizationParam> CharacterizationParams { get; set; }
        public virtual DbSet<Classification> Classifications { get; set; }
        public virtual DbSet<CompositionData> CompositionDataSet { get; set; }
        public virtual DbSet<CompositionModel> CompositionModels { get; set; }
        public virtual DbSet<CompositionParam> CompositionParams { get; set; }
        public virtual DbSet<CompositionSubstitution> CompositionSubstitutions { get; set; }
        public virtual DbSet<DataSource> DataSources { get; set; }
        public virtual DbSet<DataType> DataTypes { get; set; }
        public virtual DbSet<DependencyParam> DependencyParams { get; set; }
        public virtual DbSet<Direction> Directions { get; set; }
        public virtual DbSet<ConservationParam> ConservationParams { get; set; }
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
        public virtual DbSet<FragmentSubstitution> FragmentSubstitutions { get; set; }
        public virtual DbSet<ILCDEntity> ILCDEntities { get; set; }
        public virtual DbSet<ImpactCategory> ImpactCategories { get; set; }
        public virtual DbSet<IndicatorType> IndicatorTypes { get; set; }
        public virtual DbSet<LCIA> LCIAs { get; set; }
        public virtual DbSet<LCIAMethod> LCIAMethods { get; set; }
        public virtual DbSet<NodeCache> NodeCaches { get; set; }
        public virtual DbSet<NodeType> NodeTypes { get; set; }
        public virtual DbSet<Param> Params { get; set; }
        public virtual DbSet<ParamType> ParamTypes { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<ProcessComposition> ProcessCompositions { get; set; }
        public virtual DbSet<ProcessDissipation> ProcessDissipations { get; set; }
        public virtual DbSet<ProcessDissipationParam> ProcessDissipationParams { get; set; }
        public virtual DbSet<ProcessEmissionParam> ProcessEmissionParams { get; set; }
        public virtual DbSet<ProcessFlow> ProcessFlows { get; set; }
        public virtual DbSet<ProcessSubstitution> ProcessSubstitutions { get; set; }
        public virtual DbSet<ProcessType> ProcessTypes { get; set; }
        public virtual DbSet<ReferenceType> ReferenceTypes { get; set; }
        public virtual DbSet<Scenario> Scenarios { get; set; }
        public virtual DbSet<BackgroundSubstitution> BackgroundSubstitutions { get; set; }
        public virtual DbSet<ScenarioGroup> ScenarioGroups { get; set; }
        
        public virtual DbSet<ScoreCache> ScoreCaches { get; set; }
        public virtual DbSet<UnitConversion> UnitConversions { get; set; }
        public virtual DbSet<UnitGroup> UnitGroups { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Visibility> Visibilities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<BackgroundSubstitution>()
                .HasRequired(e => e.Scenario)
                .WithMany(e => e.BackgroundSubstitutions)
                .HasForeignKey(e => e.ScenarioID)
                .WillCascadeOnDelete(true);

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

            modelBuilder.Entity<CharacterizationParam>()
                .HasRequired(e => e.Param)
                // Unable to implement 0..1 on the other side of the relationship here
                .WithMany(e => e.CharacterizationParams) 
                .HasForeignKey(e => e.ParamID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CompositionModel>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<CompositionModel>()
                .HasMany(e => e.CompositionSubstitutions)
                .WithRequired(d => d.CompositionModel)
                .HasForeignKey(d => d.CompositionModelID);

            modelBuilder.Entity<CompositionModel>()
                .HasMany(e => e.CompositionSubstitutions)
                .WithRequired(d => d.SubstituteCompositionModel)
                .HasForeignKey(d => d.SubstituteCompositionModelID);

            modelBuilder.Entity<CompositionParam>()
                .HasRequired(e => e.Param)
                .WithMany(e => e.CompositionParams)
                .HasForeignKey(e => e.ParamID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CompositionSubstitution>()
                .HasRequired(e => e.Scenario)
                .WithMany(e => e.CompositionSubstitutions)
                .HasForeignKey(e => e.ScenarioID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ConservationParam>()
                .HasRequired(e => e.DependencyParam)
                .WithOptional(e => e.ConservationParam)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DataSource>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<DataSource>()
                .Property(e => e.DirName)
                .IsUnicode(false);

            modelBuilder.Entity<DataType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            // modelBuilder.Entity<ConservationParam>()
            //    .HasKey(e => e.BalanceParamID);

            //modelBuilder.Entity<ConservationParam>()
            //    .HasRequired(e => e.DistributionParam);
                //.WithOptional(e => e.DistributionParam);

            //modelBuilder.Entity<ConservationParam>()
            //    .HasRequired(e => e.ConservationDependencyParam)
            //    .WithMany(d => d.ConservationDistributionParams)
            //    .HasForeignKey(e => e.ConservationDependencyParamID);

            modelBuilder.Entity<DependencyParam>()
                .HasRequired(e => e.Param)
                .WithMany(e => e.DependencyParams)
                .HasForeignKey(e => e.ParamID)
                .WillCascadeOnDelete(true);

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
                .WithRequired(e => e.FlowProperty)
                .HasForeignKey(e => e.ReferenceFlowProperty);

            modelBuilder.Entity<FlowProperty>()
                .HasMany(e => e.LCIAMethods)
                .WithRequired(e => e.FlowProperty)
                .HasForeignKey(e => e.ReferenceFlowPropertyID);

            modelBuilder.Entity<FlowPropertyParam>()
                .HasRequired(e => e.Param)
                // Unable to implement 0..1 on the other side of the relationship here
                .WithMany(e => e.FlowPropertyParams) 
                .HasForeignKey(e => e.ParamID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<FlowType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Fragment>()
                .Property(e => e.Name)
                .IsUnicode(false);

            /*
            modelBuilder.Entity<Fragment>()
                .HasMany(e => e.FragmentFlows)
                .WithRequired(e => e.Fragment)
                .HasForeignKey(e => e.FragmentID);
             * */

            modelBuilder.Entity<Fragment>()
                .HasMany(e => e.FragmentNodeFragments)
                .WithRequired(e => e.SubFragment)
                .HasForeignKey(e => e.SubFragmentID);

            modelBuilder.Entity<Fragment>()
               .HasMany(e => e.FragmentSubstitutions)
               .WithRequired(e => e.SubFragment)
               .HasForeignKey(e => e.SubFragmentID);

            modelBuilder.Entity<Fragment>()
               .HasMany(e => e.Scenarios)
               .WithRequired(e => e.TopLevelFragment)
               .HasForeignKey(e => e.TopLevelFragmentID);

            modelBuilder.Entity<FragmentFlow>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FragmentFlow>()
                .HasMany(e => e.ChildFragmentFlows)
                .WithOptional(e => e.ParentFragmentFlow)
                .HasForeignKey(e => e.ParentFragmentFlowID);

            modelBuilder.Entity<FragmentStage>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FragmentSubstitution>()
                .HasRequired(e => e.Scenario)
                .WithMany(e => e.FragmentSubstitutions)
                .HasForeignKey(e => e.ScenarioID)
                .WillCascadeOnDelete(true);

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

            //modelBuilder.Entity<LCIA>()
            //    .HasMany(e => e.CharacterizationParams)
            //    .WithOptional(e => e.LCIA)
            //    .HasForeignKey(e => e.LCIAID);

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

            modelBuilder.Entity<NodeCache>()
                .HasRequired(e => e.Scenario)
                .WithMany(e => e.NodeCaches)
                .HasForeignKey(e => e.ScenarioID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<NodeType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Param>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Param>()
                .HasRequired(e => e.Scenario)
                .WithMany(e => e.Params)
                .HasForeignKey(e => e.ScenarioID)
                .WillCascadeOnDelete(true);

            //modelBuilder.Entity<Param>()
            //    .HasMany(p => p.BalanceParams)
            //    .WithRequired(b => b.BalanceParam)
            //    .HasForeignKey(b => b.BalanceParamID);

            //modelBuilder.Entity<Param>()
            //    .HasMany(p => p.DistributionParams)
            //    .WithRequired(b => b.DistributionParam)
            //    .HasForeignKey(b => b.DistributionParamID);

            modelBuilder.Entity<ParamType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.ReferenceYear)
                .IsUnicode(false);

            modelBuilder.Entity<Process>()
                .Property(e => e.Geography)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessDissipationParam>()
                .HasRequired(e => e.Param)
                .WithMany(e => e.ProcessDissipationParams) 
                .HasForeignKey(e => e.ParamID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ProcessEmissionParam>()
                .HasRequired(e => e.Param)
                .WithMany(e => e.ProcessEmissionParams) 
                .HasForeignKey(e => e.ParamID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.VarName)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessFlow>()
                .Property(e => e.Geography)
                .IsUnicode(false);

            modelBuilder.Entity<ProcessSubstitution>()
                .HasRequired(e => e.Scenario)
                .WithMany(e => e.ProcessSubstitutions)
                .HasForeignKey(e => e.ScenarioID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ProcessType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ReferenceType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Scenario>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ScoreCache>()
                .HasRequired(e => e.Scenario)
                .WithMany(e => e.ScoreCaches)
                .HasForeignKey(e => e.ScenarioID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ScenarioGroup>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UnitConversion>()
                .Property(e => e.Unit)
                .IsUnicode(false);

            modelBuilder.Entity<UnitConversion>()
                .Property(e => e.LongName)
                .IsUnicode(true);

            modelBuilder.Entity<UnitGroup>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UnitGroup>()
                .HasMany(e => e.UnitConversions)
                .WithRequired(e => e.UnitGroup)
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
