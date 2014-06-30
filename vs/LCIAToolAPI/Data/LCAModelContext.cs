using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using Data.Mappings;

namespace Data
{
    public class LCAModelContext : DbContext, IDbContext
    {
         static LCAModelContext()
    {
        Database.SetInitializer<LCAModelContext>(null);
    }
 
    public LCAModelContext()
             : base("Name=LCAModelContext")
    {
    }

    public new IDbSet<T> Set<T>() where T : class
    {
        return base.Set<T>();
    }

    public override int SaveChanges()
    {
        
        return base.SaveChanges();
    }


    public DbSet<CompositionData> CompositionDatas { get; set; }
    public DbSet<CompositionParam> CompositionParams { get; set; }
    public DbSet<FlowPropertyParam> FlowPropertyParams { get; set; }
    public DbSet<Background> Backgrounds { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategorySystem> CategorySystems { get; set; }
    public DbSet<CharacterizationParam> CharacterizationParams { get; set; }
    public DbSet<Classification> Classifications { get; set; }
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
    public DbSet<FlowType> FlowTypes { get; set; }
    public DbSet<Fragment> Fragments { get; set; }
    public DbSet<FragmentFlow> FragmentFlows { get; set; }
    public DbSet<FragmentNodeProcess> FragmentNodeProcesses { get; set; }
    public DbSet<FragmentStage> FragmentStages { get; set; }
    public DbSet<ILCDEntity> ILCDEntities { get; set; }
    public DbSet<ImpactCategory> ImpactCategories { get; set; }
    public DbSet<IndicatorType> IndicatorTypes { get; set; }
    public DbSet<LCIA> LCIAs { get; set; }
    public DbSet<LCIAMethod> LCIAMethods { get; set; }
    public DbSet<NodeEmissionParam> NodeEmissionParams { get; set; }
    public DbSet<NodeType> NodeTypes { get; set; }
    public virtual DbSet<Param> Params { get; set; }
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
    public DbSet<UnitConversion> UnitConversions { get; set; }
    public DbSet<UnitGroup> UnitGroups { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Visibility> Visibilities { get; set; }


    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScenarioParam>().HasRequired(scenarioParam => scenarioParam.Param)
                                   .WithRequiredDependent();
        modelBuilder.Entity<FragmentStage>().HasRequired(scenarioParam => scenarioParam.Fragment)
                                  .WithRequiredDependent();
    }
}
    
}
