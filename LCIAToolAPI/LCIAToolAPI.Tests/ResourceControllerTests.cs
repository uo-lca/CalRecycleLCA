using CalRecycleLCA.Services;
using Entities.Models;
using LcaDataModel;
using LCAToolAPI.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace LCIAToolAPI.Tests
{
    /// <summary>
    /// this concentrates on testing api end points as requested
    /// new development should test each unit of work at the service level
    /// </summary>
    [TestClass]
    public class ResourceControllerTests
    {

        private ICategoryService _categoryService;
        private IClassificationService _classificationService;
        private IFragmentService _fragmentService;
        private IFragmentFlowService _fragmentFlowService;
        private IFragmentStageService _fragmentStageService;
        private IFlowService _flowService;
        private IFlowPropertyService _flowPropertyService;
        private FlowTypeService _flowTypeService;
        private IImpactCategoryService _impactCategoryService;
        private ILCIAMethodService _lciaMethodService;
        private IProcessService _processService;
        private IProcessFlowService _processFlowService;
        private IScenarioService _scenarioService;
        private IScenarioGroupService _scenarioGroupService;
        private INodeCacheService _nodeCacheService;
        //private IFragmentNodeProcessService _fragmentNodeProcessService;
        //private IFragmentNodeFragmentService _fragmentNodeFragmentService;
        private IFlowFlowPropertyService _flowFlowPropertyService;
        private IDependencyParamService _dependencyParamService;
        //private IFlowPropertyParamService _flowPropertyParamService;
        private IParamService _paramService;
        private IScoreCacheService _scoreCacheService;
        private IProcessSubstitutionService _processSubstitutionService;
        private IFragmentSubstitutionService _fragmentSubstitutionService;
        //private IBackgroundSubstitutionService _backgroundSubstitutionService;
        private IBackgroundService _backgroundService;
        //private IProcessEmissionParamService _processEmissionParamService;
        private IFlowPropertyEmissionService _flowPropertyEmissionService;
        private IProcessDissipationService _processDissipationService;
        //private IProcessDissipationParamService _processDissipationParamService;
        private ILCIAService _lciaService;
        //private ICharacterizationParamService _characterizationParamService;

        private IUnitOfWork _unitOfWork;

        private IFragmentTraversalV2 _fragmentTraversalV2;
        private IFragmentLCIAComputation _fragmentLCIAComputation;
        private ILCIAComputationV2 _lciaComputation;
        private ResourceServiceFacade _resourceServiceFacade;
        private ResourceController _resourceController;
        private DocuService _docuService;

        private List<DataSource> _dataSources;
        private List<FlowType> _flowTypes;
        private LCIAResultResource _processLCIAResult;
        private LCIAMethod _lciaMethod;
        private List<Scenario> _scenarios;
        private List<Fragment> _fragments;
        private Fragment _fragment;
        private List<FragmentFlow> _fragmentFlows;
        private List<Flow> _flows;
        private List<Process> _processes;
        private List<ILCDEntity> _ilcdEntities;
        
        private Mock<IRepositoryAsync<Category>> _mockCategoryRepository;
        private Mock<IRepositoryAsync<Classification>> _mockClassificationRepository;
        private Mock<IRepositoryAsync<Fragment>> _mockFragmentRepository;
        private Mock<IRepositoryAsync<FragmentFlow>> _mockFragmentFlowRepository;
        private Mock<IRepositoryAsync<FragmentStage>> _mockFragmentStageRepository;
        private Mock<IRepositoryAsync<Flow>> _mockFlowRepository;
        private Mock<IRepositoryAsync<FlowProperty>> _mockFlowPropertyRepository;
        private Mock<IRepositoryAsync<FlowType>> _mockFlowTypeRepository;
        private Mock<IRepositoryAsync<ImpactCategory>> _mockImpactCategoryRepository;
        private Mock<IRepositoryAsync<LCIAMethod>> _mockLCIAMethodRepository;
        private Mock<IRepositoryAsync<Process>> _mockProcessRepository;
        private Mock<IRepositoryAsync<ProcessFlow>> _mockProcessFlowRepository;
        private Mock<IRepositoryAsync<Scenario>> _mockScenarioRepository;
        private Mock<IRepositoryAsync<ScenarioGroup>> _mockScenarioGroupRepository;
        private Mock<IRepositoryAsync<NodeCache>> _mockNodeCacheRepository;
        private Mock<IRepositoryAsync<FragmentNodeProcess>> _mockFragmentNodeProcessRepository;
        private Mock<IRepositoryAsync<FragmentNodeFragment>> _mockFragmentNodeFragmentRepository;
        private Mock<IRepositoryAsync<FlowFlowProperty>> _mockFlowFlowPropertyRepository;
        private Mock<IRepositoryAsync<DependencyParam>> _mockDependencyParamRepository;
        private Mock<IRepositoryAsync<FlowPropertyParam>> _mockFlowPropertyParamRepository;
        private Mock<IRepositoryAsync<Param>> _mockParamRepository;
        private Mock<IRepositoryAsync<ScoreCache>> _mockScoreCacheRepository;
        private Mock<IRepositoryAsync<ProcessSubstitution>> _mockProcessSubstitutionRepository;
        private Mock<IRepositoryAsync<FragmentSubstitution>> _mockFragmentSubstitutionRepository;
        private Mock<IRepositoryAsync<BackgroundSubstitution>> _mockBackgroundSubstitutionRepository;
        private Mock<IRepositoryAsync<Background>> _mockBackgroundRepository;
        private Mock<IRepositoryAsync<ProcessEmissionParam>> _mockProcessEmissionParamRepository;
        private Mock<IRepositoryAsync<FlowPropertyEmission>> _mockFlowPropertyEmissionRepository;
        private Mock<IRepositoryAsync<ProcessDissipation>> _mockProcessDissipationRepository;
        private Mock<IRepositoryAsync<ProcessDissipationParam>> _mockProcessDissipationParamRepository;
        private Mock<IRepositoryAsync<LCIA>> _mockLCIARepository;
        private Mock<IRepositoryAsync<CharacterizationParam>> _mockCharacterizationParamRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;


        [TestInitialize]
        public void SetUp()
        {
            _mockCategoryRepository = new Mock<IRepositoryAsync<Category>>();
            _mockClassificationRepository = new Mock<IRepositoryAsync<Classification>>();
            _mockFlowRepository = new Mock<IRepositoryAsync<Flow>>();
            _mockFlowPropertyRepository = new Mock<IRepositoryAsync<FlowProperty>>();
            _mockFlowTypeRepository = new Mock<IRepositoryAsync<FlowType>>();
            _mockFragmentRepository = new Mock<IRepositoryAsync<Fragment>>();
            _mockFragmentFlowRepository = new Mock<IRepositoryAsync<FragmentFlow>>();
            _mockFragmentStageRepository = new Mock<IRepositoryAsync<FragmentStage>>();
            _mockImpactCategoryRepository = new Mock<IRepositoryAsync<ImpactCategory>>();
            _mockLCIAMethodRepository = new Mock<IRepositoryAsync<LCIAMethod>>();
            _mockNodeCacheRepository = new Mock<IRepositoryAsync<NodeCache>>();
            _mockProcessRepository = new Mock<IRepositoryAsync<Process>>();
            _mockProcessFlowRepository = new Mock<IRepositoryAsync<ProcessFlow>>();
            _mockScenarioRepository = new Mock<IRepositoryAsync<Scenario>>();
            _mockScenarioGroupRepository = new Mock<IRepositoryAsync<ScenarioGroup>>();
            _mockFragmentNodeProcessRepository = new Mock<IRepositoryAsync<FragmentNodeProcess>>();
            _mockFragmentNodeFragmentRepository = new Mock<IRepositoryAsync<FragmentNodeFragment>>();
            _mockFlowFlowPropertyRepository = new Mock<IRepositoryAsync<FlowFlowProperty>>();
            _mockDependencyParamRepository = new Mock<IRepositoryAsync<DependencyParam>>();
            _mockFlowPropertyParamRepository = new Mock<IRepositoryAsync<FlowPropertyParam>>();
            _mockParamRepository = new Mock<IRepositoryAsync<Param>>();
            _mockScoreCacheRepository = new Mock<IRepositoryAsync<ScoreCache>>();
            _mockProcessSubstitutionRepository = new Mock<IRepositoryAsync<ProcessSubstitution>>();
            _mockFragmentSubstitutionRepository = new Mock<IRepositoryAsync<FragmentSubstitution>>();
            _mockBackgroundSubstitutionRepository = new Mock<IRepositoryAsync<BackgroundSubstitution>>();
            _mockBackgroundRepository = new Mock<IRepositoryAsync<Background>>();
            _mockProcessEmissionParamRepository = new Mock<IRepositoryAsync<ProcessEmissionParam>>();
            _mockFlowPropertyEmissionRepository = new Mock<IRepositoryAsync<FlowPropertyEmission>>();
            _mockProcessDissipationRepository = new Mock<IRepositoryAsync<ProcessDissipation>>();
            _mockProcessDissipationParamRepository = new Mock<IRepositoryAsync<ProcessDissipationParam>>();
            _mockLCIARepository = new Mock<IRepositoryAsync<LCIA>>();
            _mockCharacterizationParamRepository = new Mock<IRepositoryAsync<CharacterizationParam>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            //we do need to pass a mock repository to all these services as the ResourceController 
            //expects the resource facade to be passed to it.  In turn the resource facade takes these 
            //service types and methods.
            _flowTypeService = new FlowTypeService(_mockFlowTypeRepository.Object);
            _categoryService = new CategoryService(_mockCategoryRepository.Object);
            _classificationService = new ClassificationService(_mockClassificationRepository.Object);
            _fragmentService = new FragmentService(_mockFragmentRepository.Object);
            _fragmentFlowService = new FragmentFlowService(_mockFragmentFlowRepository.Object);
            _fragmentStageService = new FragmentStageService(_mockFragmentStageRepository.Object);
            _flowService = new FlowService(_mockFlowRepository.Object);
            _flowPropertyService = new FlowPropertyService(_mockFlowPropertyRepository.Object);
            _impactCategoryService = new ImpactCategoryService(_mockImpactCategoryRepository.Object);
            _lciaMethodService = new LCIAMethodService(_mockLCIAMethodRepository.Object);
            _processService = new ProcessService(_mockProcessRepository.Object);
            _processFlowService = new ProcessFlowService(_mockProcessFlowRepository.Object);
            _scenarioService = new ScenarioService(_mockScenarioRepository.Object);
            _scenarioGroupService = new ScenarioGroupService(_mockScenarioGroupRepository.Object);
            _nodeCacheService = new NodeCacheService(_mockNodeCacheRepository.Object);
            //_fragmentNodeProcessService = new FragmentNodeProcessService(_mockFragmentNodeProcessRepository.Object);
            //_fragmentNodeFragmentService = new FragmentNodeFragmentService(_mockFragmentNodeFragmentRepository.Object);
            _flowFlowPropertyService = new FlowFlowPropertyService(_mockFlowFlowPropertyRepository.Object);
            _dependencyParamService = new DependencyParamService(_mockDependencyParamRepository.Object);
            //_flowPropertyParamService = new FlowPropertyParamService(_mockFlowPropertyParamRepository.Object);
            //_flowPropertyParamService = new FlowPropertyParamService(_mockFlowPropertyParamRepository.Object);
            _paramService = new ParamService(_mockParamRepository.Object);
            _scoreCacheService = new ScoreCacheService(_mockScoreCacheRepository.Object);
            _processSubstitutionService = new ProcessSubstitutionService(_mockProcessSubstitutionRepository.Object);
            _fragmentSubstitutionService = new FragmentSubstitutionService(_mockFragmentSubstitutionRepository.Object);
            //_backgroundSubstitutionService = new BackgroundSubstitutionService(_mockBackgroundSubstitutionRepository.Object);
            _backgroundService = new BackgroundService(_mockBackgroundRepository.Object);
            //_processEmissionParamService = new ProcessEmissionParamService(_mockProcessEmissionParamRepository.Object);
            _flowPropertyEmissionService = new FlowPropertyEmissionService(_mockFlowPropertyEmissionRepository.Object);
            _processDissipationService = new ProcessDissipationService(_mockProcessDissipationRepository.Object);
            //_processDissipationParamService = new ProcessDissipationParamService(_mockProcessDissipationParamRepository.Object);
            _lciaService = new LCIAService(_mockLCIARepository.Object);
            //_characterizationParamService = new CharacterizationParamService(_mockCharacterizationParamRepository.Object);
            _unitOfWork = _mockUnitOfWork.Object;

            //methods that resource service facade is dependent on.
            _fragmentTraversalV2 = new FragmentTraversalV2(
                _fragmentFlowService,
                _nodeCacheService,
                _processFlowService,
                _flowFlowPropertyService,
                _dependencyParamService);

            _lciaComputation = new LCIAComputationV2(_processFlowService,
                //_processEmissionParamService,
            _lciaMethodService,
                //_flowService,
                //_flowFlowPropertyService,
                //_flowPropertyParamService,
                //_flowPropertyEmissionService,
                //_processDissipationService,
                //_processDissipationParamService,
            _lciaService);
            //_characterizationParamService,
            //_paramService);

            _fragmentLCIAComputation = new FragmentLCIAComputation(_fragmentTraversalV2,
                _lciaComputation,
                _fragmentFlowService,
                _scoreCacheService,
                _nodeCacheService,
            //_fragmentNodeProcessService,
            //_processSubstitutionService,
            //_fragmentNodeFragmentService,
            //_fragmentSubstitutionService,
                _lciaMethodService,
                _fragmentService,
            _unitOfWork);

            _resourceServiceFacade = new ResourceServiceFacade(
                               _classificationService,
                               _fragmentService,
                               _fragmentFlowService,
                               _fragmentStageService,
                               _fragmentLCIAComputation,
                               _flowService,
                               _flowPropertyService,
                               _flowTypeService,
                               _impactCategoryService,
                               _lciaComputation,
                               _lciaMethodService,
                               _processService,
                               _processFlowService,
                               _scenarioService, 
                               _nodeCacheService, 
                               _scoreCacheService,
                               _paramService,
                               _flowFlowPropertyService,
                               _lciaService,
                               _unitOfWork);

            _docuService = new DocuService();

            _resourceController = new ResourceController(_resourceServiceFacade, _scenarioGroupService, _docuService);


        }



        [TestMethod]
        public void ShouldSuccesfullyCompareOnCount()
        {
            // Arrrange - This will populate our "mock" repository as a use case to
            //compare again what we actually recieve from the method.
            _flowTypes = new List<FlowType>
                     {
                            new FlowType(){FlowTypeID=1,Name="IntermediateFlow"},
                            new FlowType(){FlowTypeID=2,Name="ElementaryFlow"}

                     }.ToList();

            //needs to be converted into a FlowTypeResource type as thing is what the method will return
            List<FlowTypeResource> _flowTypeResources = _flowTypes
   .Select(x => new FlowTypeResource() { FlowTypeID = x.FlowTypeID, Name = x.Name })
   .ToList();

            //We only set up the mock repository that we need
            _mockFlowTypeRepository.Setup(m => m.Queryable()).Returns(_flowTypes.AsQueryable());


            //Act
            List<FlowTypeResource> result = _resourceController.GetFlowTypes().ToList();
            Assert.AreEqual(_flowTypeResources.Count, result.Count);
        }

        [TestMethod]
        public void ShouldSuccesfullyCompareReturnedFlowTypes()
        {
            // Arrrange - This will populate our "mock" repository as a use case to
            //compare again what we actually recieve from the method.
            _flowTypes = new List<FlowType>
                     {
                            new FlowType(){FlowTypeID=1,Name="IntermediateFlow"},
                            new FlowType(){FlowTypeID=2,Name="ElementaryFlow"}

                     }.ToList();

            //needs to be converted into a FlowTypeResource type as thing is what the method will return
            List<FlowTypeResource> _flowTypeResources = _flowTypes
   .Select(x => new FlowTypeResource() { FlowTypeID = x.FlowTypeID, Name = x.Name })
   .ToList();

            //We only set up the mock repository that we need
            _mockFlowTypeRepository.Setup(m => m.Queryable()).Returns(_flowTypes.AsQueryable());


            //Act
            List<FlowTypeResource> result = _resourceController.GetFlowTypes().ToList();
            //CollectionAssert.AreEquivalent(_flowTypeResources, result);
            //Assert.IsTrue(_flowTypeResources.SequenceEqual(result));
            Assert.AreEqual(_flowTypeResources[0].FlowTypeID, result[0].FlowTypeID);
            Assert.AreEqual(_flowTypeResources[1].FlowTypeID, result[1].FlowTypeID); 
        }

        [TestMethod]
        //In progress - I think I know why it doesn't pass now.
        public void ShouldSuccesfullyGetProcessLCIAResult()
        {
            int processID = 1; 
            int lciaMethodID = 1;
            int scenarioID = Scenario.MODEL_BASE_CASE_ID;

            //We only set up the mock repositories that we need
            _lciaMethod = new LCIAMethod()
            {
                LCIAMethodID = 1,
                Name = "ILCD2011; Non-cancer human health effects; midpoint; CTUh; USEtox",
                Methodology = "ILCD2011",
                ImpactCategoryID =10,
                ImpactIndicator = "Comparative Toxic Unit for human (CTUh)expressing the estimated increase in morbidity in the total human population per unit mass of a chemical emitted (cases per kilogramme).",
                ReferenceYear = "time independent",
                Duration="Indefinite",
                ImpactLocation="GLO",
                IndicatorTypeID=1,
                Normalization=false,
                Weighting=false,
                UseAdvice = "Recommended to do a sensitivity analysis by separately calculating the human toxicity results using (i) recommended and interim characterization factors, and (ii) recommended characterization factors only. No factors are listed for particulate matter as there is a specific impact category for that.",
                ReferenceFlowPropertyID=222,
                ILCDEntityID=1756
            };

            //needs to be converted into a LCIAMethodResource type as this is what the method will return
            LCIAMethodResource _lciaMethodResources = new LCIAMethodResource();
            _lciaMethodResources.LCIAMethodID = _lciaMethod.LCIAMethodID;
            _lciaMethodResources.Name = _lciaMethod.Name;
            _lciaMethodResources.Methodology = _lciaMethod.Methodology;
            _lciaMethodResources.ImpactCategoryID = Convert.ToInt32(_lciaMethod.ImpactCategoryID);
            _lciaMethodResources.ImpactIndicator = _lciaMethod.ImpactIndicator;
            _lciaMethodResources.ReferenceYear = _lciaMethod.ReferenceYear;
            _lciaMethodResources.Duration = _lciaMethod.Duration;
            _lciaMethodResources.ImpactLocation = _lciaMethod.ImpactLocation;
            _lciaMethodResources.Normalization = Convert.ToBoolean(_lciaMethod.Normalization);
            _lciaMethodResources.Weighting = Convert.ToBoolean(_lciaMethod.Weighting);
            _lciaMethodResources.UseAdvice = _lciaMethod.UseAdvice;


            _mockLCIAMethodRepository.Setup(m => m.Find(lciaMethodID)).Returns(_lciaMethod);

            //We only set up the mock repositories that we need

            _dataSources = new List<DataSource>
            {
                new DataSource() {
                    DataSourceID = 4,
                    Name = "Mock Data Source",
                    VisibilityID = 1
                }
            };

            _ilcdEntities = new List<ILCDEntity>
            {
                new ILCDEntity() {
                    ILCDEntityID = 1,
                    UUID = "01c96a9f-aeb1-4c5f-bc36-5de9638799f9",
                    Version	= "00.00.000",
                    DataSourceID = 4,
                    DataTypeID = 3
                }
            };
            
            _processes = new List<Process>
                     {
                            new Process(){   ProcessID = 1,
                Name = "Metal Emissions, DK MDO",
                ReferenceYear = "2013",
                Geography = "US",
                ReferenceTypeID = 1,
                ProcessTypeID = 5,
                ReferenceFlowID = null,
                ILCDEntityID = 1}

                     }.ToList();

            //needs to be converted into a LCIAMethodResource type as this is what the method will return
            List<ProcessResource> _processResources = _processes
                   .Select(x => new ProcessResource() { 
                       ProcessID = x.ProcessID, 
                       Name = x.Name,
                       ReferenceYear = x.ReferenceYear,
                       Geography = x.Geography,
                       ReferenceTypeID = x.ReferenceTypeID,
                       //ProcessTypeID = Convert.ToInt32(x.ProcessTypeID),
                       ReferenceFlowID=x.ReferenceFlowID
                   }).ToList();


            _mockProcessRepository.Setup(m => m.Queryable()).Returns(_processes.AsQueryable());
            //_mockIlcdEntityRepository.Setup(m => m.Queryable()).Returns(_ilcdEntities.AsQueryable());
            //_mockDataSourceRepository.Setup(m => m.Queryable()).Returns(_dataSources.AsQueryable());
            _processService = new ProcessService(_mockProcessRepository.Object);
            //Act
            _processService.IsPrivate(processID); // do not know how to accomplish this query with a mock repository layer
            //Assert
            _mockProcessRepository.Verify();

    //        _mockProcessRepository.As<IRepositoryAsync<Process>>()
    //.Setup( x => x.Queryable.() )
    //.Returns( Task.FromResult( userData.ToList() ) );




              _processLCIAResult = new LCIAResultResource()
                     {
                                LCIAMethodID=1,
                                ScenarioID = Scenario.MODEL_BASE_CASE_ID, 
                                LCIAScore = new List<AggregateLCIAResource>
                                   {
                                      new AggregateLCIAResource()
                                         {
                                              ProcessID=1,
                                              LCIADetail = new List<DetailedLCIAResource>
                                                {
                                                     new DetailedLCIAResource()
                                                     {
                                                          FlowID = 168,
                                                          Direction = "Output",
                                                          Quantity = 3E-06,
                                                          Factor = 0.000171,
                                                          Result = 5.1300000000000009E-10
                                                     },
                                                      new DetailedLCIAResource()
                                                     {
                                                          FlowID = 398,
                                                          Direction = "Output",
                                                          Quantity = 6.93E-07,
                                                          Factor = 0.0168,
                                                          Result = 1.16424E-08
                                                     },
                                                      new DetailedLCIAResource()
                                                     {
                                                          FlowID = 1014,
                                                          Direction = "Output",
                                                          Quantity = 1.35E-06,
                                                          Factor = 4.19E-05,
                                                          Result = 5.6565E-11
                                                     }
   
                                                }
                                          }
                                    }
                            
                     
                     
                     };

            //Act
            LCIAResultResource result = _resourceController.GetProcessLCIAResult(processID, lciaMethodID, scenarioID);
            Assert.AreEqual(_processLCIAResult.LCIAMethodID, result.LCIAMethodID); 



        }

        [TestMethod]
        public void ShouldSuccesfullyCompareReturnedScenarios()
        {
            // Arrrange - This will populate our "mock" repository as a use case to
            //compare again what we actually recieve from the method.
            _scenarios = new List<Scenario>
                     {
                            new Scenario(){ScenarioID=Scenario.MODEL_BASE_CASE_ID, Name="Model Base Case"},
                            new Scenario(){ScenarioID=2,Name="Process Substitution"}

                     }.ToList();

            //needs to be converted into a ScenarioResource type as thing is what the method will return
            List<ScenarioResource> _scenarioResources = _scenarios
   .Select(x => new ScenarioResource() { ScenarioID = x.ScenarioID, Name = x.Name })
   .ToList();

            //We only set up the mock repository that we need
            _mockScenarioRepository.Setup(m => m.Queryable()).Returns(_scenarios.AsQueryable());

            //Act
            List<ScenarioResource> result = _resourceController.GetScenarios().ToList();
            Assert.AreEqual(_scenarioResources[0].Name, result[0].Name);
            Assert.AreEqual(_scenarioResources[1].Name, result[1].Name);
        }

        [TestMethod]
        public void ShouldSuccesfullyCompareReturnedFragments()
        {
            // Arrrange - This will populate our "mock" repository as a use case to
            //compare again what we actually recieve from the method.
            _fragments = new List<Fragment>
                     {
                            new Fragment(){FragmentID=5,Name="Haz Waste Incineration Output"},
                            new Fragment(){FragmentID=9,Name="Used oil in Wastewater treatment plant"}

                     }.ToList();

            //needs to be converted into a FlowTypeResource type as thing is what the method will return
            List<FragmentResource> _fragmentResources = _fragments
   .Select(x => new FragmentResource() { FragmentID = x.FragmentID, Name = x.Name })
   .ToList();

            //We only set up the mock repository that we need
            _mockFragmentRepository.Setup(m => m.Queryable()).Returns(_fragments.AsQueryable());


            //Act
            List<FragmentResource> result = _resourceController.GetFragments().ToList();
            Assert.AreEqual(_fragmentResources[0].Name, result[0].Name);
            Assert.AreEqual(_fragmentResources[1].Name, result[1].Name);
        }

        [TestMethod]
        public void ShouldSuccesfullyCompareReturnedFragmentByFragmentID()
        {
            int fragmentID = 1;

            // Arrrange - This will populate our "mock" repository as a use case to
            //compare again what we actually recieve from the method.
            //We only set up the mock repositories that we need
            _fragment = new Fragment()
            {
                FragmentID = 1,
                Name = "Electricity, at grid"
            };


           
            //We only set up the mock repository that we need
            _mockFragmentRepository.Setup(m => m.Find(fragmentID)).Returns(_fragment);

            //Act
            FragmentResource result = _resourceController.GetFragment(fragmentID);
            Assert.AreEqual(_fragment.Name, result.Name);
        }

        [TestMethod]
        //In progress - I think I know why it doesn't pass now.
        public void ShouldSuccesfullyCompareReturnedFragmentFlowByFragmentID()
        {
            int fragmentID = 1;

            // Arrrange - This will populate our "mock" repository as a use case to
            //compare again what we actually recieve from the method.
            _fragmentFlows = new List<FragmentFlow>
                     {
                            new FragmentFlow(){ FragmentFlowID=1, FragmentID=1, Name="Electricity, at grid", ShortName="Electricity, at grid", FragmentStageID=null, NodeTypeID=1, FlowID=null, DirectionID=1, ParentFragmentFlowID=null},
                            new FragmentFlow(){ FragmentFlowID=2, FragmentID=1, Name="Electricity, at grid, CA", ShortName="Electricity, at grid, CA", FragmentStageID=null, NodeTypeID=1, FlowID=687, DirectionID=1, ParentFragmentFlowID=1}
                     }.ToList();

            //needs to be converted into a FlowTypeResource type as thing is what the method will return
            List<FragmentFlowResource> _fragmentFlowResources = _fragmentFlows
   .Select(x => new FragmentFlowResource() { FragmentFlowID = x.FragmentFlowID, Name = x.Name })
   .ToList();

            //We only set up the mock repository that we need
            _mockFragmentFlowRepository.Setup(m => m.Queryable()).Returns(_fragmentFlows.AsQueryable());

            //Act
            List<FragmentFlowResource> result = _resourceController.GetFragmentFlowResources(fragmentID).ToList();
            Assert.AreEqual(_fragmentFlowResources[0].Name, result[0].Name);
            Assert.AreEqual(_fragmentFlowResources[1].Name, result[1].Name);
        }

        [TestMethod]
        public void ShouldSuccesfullyCompareReturnedFlowsByFragmentID()
        {
            int fragmentID = 1;

            // Arrrange - This will populate our "mock" repository as a use case to
            //compare again what we actually recieve from the method.
            //We only set up the mock repositories that we need
            _flows = new List<Flow>
                     {
                            new Flow(){FlowID=1, Name="Electricity from lignite"},
                            new Flow(){FlowID=5, Name="UO_Light distillates"},
                            new Flow(){FlowID=9, Name="platinum"},

                     }.ToList();

            List<FlowResource> _flowResources = _flows
.Select(x => new FlowResource() { FlowID = x.FlowID, Name = x.Name })
.ToList();

            //We only set up the mock repository that we need
            _mockFlowRepository.Setup(m => m.Queryable()).Returns(_flows.AsQueryable());

            //Act
            List<FlowResource> result = _resourceController.GetFlowsByFragment(fragmentID).ToList();
            Assert.AreEqual(_flowResources[0].Name, result[0].Name);
            Assert.AreEqual(_flowResources[1].Name, result[1].Name);
            Assert.AreEqual(_flowResources[2].Name, result[2].Name);
        }


    }


}
