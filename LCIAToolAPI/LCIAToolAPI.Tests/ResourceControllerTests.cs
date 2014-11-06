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
    [TestClass]
    public class ResourceControllerTests
    {
 
        private ICategoryService _categoryService;
        private IClassificationService _classificationService;
        private IFragmentService _fragmentService;
        private IFragmentFlowService _fragmentFlowService;
        private IFlowService _flowService;
        private IFlowPropertyService _flowPropertyService;
        private FlowTypeService _flowTypeService;
        private IImpactCategoryService _impactCategoryService;
        private ILCIAMethodService _lciaMethodService;
        private IProcessService _processService;
        private IProcessFlowService _processFlowService;
        private IScenarioService _scenarioService;
        private INodeCacheService _nodeCacheService;
        private IFragmentNodeProcessService _fragmentNodeProcessService;
        private IFragmentNodeFragmentService _fragmentNodeFragmentService;
        private IFlowFlowPropertyService _flowFlowPropertyService;
        private IDependencyParamService _dependencyParamService;
        private IFlowPropertyParamService _flowPropertyParamService;
        private IParamService _paramService;   
        private IScoreCacheService _scoreCacheService;
        private IProcessSubstitutionService _processSubstitutionService;
        private IFragmentSubstitutionService _fragmentSubstitutionService;
        // private IScenarioBackgroundService _scenarioBackgroundService;
        private IBackgroundService _backgroundService;
        private IProcessEmissionParamService _processEmissionParamService;
        private IFlowPropertyEmissionService _flowPropertyEmissionService;
        private IProcessDissipationService _processDissipationService;
        private IProcessDissipationParamService _processDissipationParamService;
        private ILCIAService _lciaService;
        private ICharacterizationParamService _characterizationParamService;

        private IUnitOfWork _unitOfWork;

        private IFragmentTraversalV2 _fragmentTraversalV2;
        private IFragmentLCIAComputation _fragmentLCIAComputation;
        private ILCIAComputationV2 _lciaComputation;
        private ResourceServiceFacade _resourceServiceFacade;
        private ResourceController _resourceController;

        private List<FlowType> _flowTypes;


        private Mock<IRepositoryAsync<Category>> _mockCategoryRepository;
        private Mock<IRepositoryAsync<Classification>> _mockClassificationRepository;
        private Mock<IRepositoryAsync<Fragment>> _mockFragmentRepository;
        private Mock<IRepositoryAsync<FragmentFlow>> _mockFragmentFlowRepository;
        private Mock<IRepositoryAsync<Flow>> _mockFlowRepository;
        private Mock<IRepositoryAsync<FlowProperty>> _mockFlowPropertyRepository;
        private Mock<IRepositoryAsync<FlowType>> _mockFlowTypeRepository;
        private Mock<IRepositoryAsync<ImpactCategory>> _mockImpactCategoryRepository;
        private Mock<IRepositoryAsync<LCIAMethod>> _mockLCIAMethodRepository;
        private Mock<IRepositoryAsync<Process>> _mockProcessRepository;
        private Mock<IRepositoryAsync<ProcessFlow>> _mockProcessFlowRepository;
        private Mock<IRepositoryAsync<Scenario>> _mockScenarioRepository;
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
        // private Mock<IRepositoryAsync<BackgroundSubstitution>> _mockScenarioBackgroundRepository;
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
            _mockFlowTypeRepository = new Mock<IRepositoryAsync<FlowType>>();
            _mockCategoryRepository = new Mock<IRepositoryAsync<Category>>();
            _mockClassificationRepository = new Mock<IRepositoryAsync<Classification>>();
            _mockFragmentRepository = new Mock<IRepositoryAsync<Fragment>>();
            _mockFragmentFlowRepository = new Mock<IRepositoryAsync<FragmentFlow>>();
            _mockFlowRepository = new Mock<IRepositoryAsync<Flow>>();
            _mockFlowPropertyRepository = new Mock<IRepositoryAsync<FlowProperty>>();
            _mockImpactCategoryRepository = new Mock<IRepositoryAsync<ImpactCategory>>();
            _mockLCIAMethodRepository = new Mock<IRepositoryAsync<LCIAMethod>>();
            _mockProcessRepository = new Mock<IRepositoryAsync<Process>>();
            _mockProcessFlowRepository = new Mock<IRepositoryAsync<ProcessFlow>>();
            _mockScenarioRepository = new Mock<IRepositoryAsync<Scenario>>();
            _mockNodeCacheRepository = new Mock<IRepositoryAsync<NodeCache>>();
            _mockFragmentNodeProcessRepository = new Mock<IRepositoryAsync<FragmentNodeProcess>>();
            _mockFragmentNodeFragmentRepository = new Mock<IRepositoryAsync<FragmentNodeFragment>>();
            _mockFlowFlowPropertyRepository = new Mock<IRepositoryAsync<FlowFlowProperty>>();
            _mockDependencyParamRepository = new Mock<IRepositoryAsync<DependencyParam>>();
            _mockFlowPropertyParamRepository = new Mock<IRepositoryAsync<FlowPropertyParam>>();
            _mockParamRepository = new Mock<IRepositoryAsync<Param>>();
            _mockScoreCacheRepository = new Mock<IRepositoryAsync<ScoreCache>>();
            _mockProcessSubstitutionRepository = new Mock<IRepositoryAsync<ProcessSubstitution>>();
            _mockFragmentSubstitutionRepository = new Mock<IRepositoryAsync<FragmentSubstitution>>();
            // _mockScenarioBackgroundRepository = new Mock<IRepositoryAsync<BackgroundSubstitution>>();
            _mockBackgroundRepository = new Mock<IRepositoryAsync<Background>>();
            _mockProcessEmissionParamRepository = new Mock<IRepositoryAsync<ProcessEmissionParam>>();
            _mockFlowPropertyEmissionRepository = new Mock<IRepositoryAsync<FlowPropertyEmission>>();
            _mockProcessDissipationRepository = new Mock<IRepositoryAsync<ProcessDissipation>>();
            _mockProcessDissipationParamRepository = new Mock<IRepositoryAsync<ProcessDissipationParam>>();
            _mockLCIARepository = new Mock<IRepositoryAsync<LCIA>>();
            _mockCharacterizationParamRepository = new Mock<IRepositoryAsync<CharacterizationParam>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
         
        }

       [TestMethod]
        public void ShouldSuccesfullyCompareOnCount()
        {
            // Arrrange
            _flowTypes = new List<FlowType>
                     {
                            new FlowType(){FlowTypeID=1,Name="IntermediateFlow"},
                            new FlowType(){FlowTypeID=2,Name="ElementaryFlow"}

                     }.ToList();

            List<FlowTypeResource> _flowTypeResources = _flowTypes
   .Select(x => new FlowTypeResource() { FlowTypeID = x.FlowTypeID, Name = x.Name })
   .ToList();

            List<FlowTypeResource> _flowTypeResources1 = _flowTypes
  .Select(x => new FlowTypeResource() { FlowTypeID = x.FlowTypeID, Name = x.Name })
  .ToList();

            _mockFlowTypeRepository.Setup(m => m.Queryable()).Returns(_flowTypes.AsQueryable());

            _flowTypeService = new FlowTypeService(_mockFlowTypeRepository.Object);
            _categoryService = new CategoryService(_mockCategoryRepository.Object);
            _classificationService = new ClassificationService(_mockClassificationRepository.Object);
            _fragmentService = new FragmentService(_mockFragmentRepository.Object);
            _fragmentFlowService = new FragmentFlowService(_mockFragmentFlowRepository.Object);
            _flowService = new FlowService(_mockFlowRepository.Object);
            _flowPropertyService = new FlowPropertyService(_mockFlowPropertyRepository.Object);
            _impactCategoryService = new ImpactCategoryService(_mockImpactCategoryRepository.Object);
            _lciaMethodService = new LCIAMethodService(_mockLCIAMethodRepository.Object);
            _processService = new ProcessService(_mockProcessRepository.Object);
            _processFlowService = new ProcessFlowService(_mockProcessFlowRepository.Object);
            _scenarioService = new ScenarioService(_mockScenarioRepository.Object);
            _nodeCacheService = new NodeCacheService(_mockNodeCacheRepository.Object);
            _fragmentNodeProcessService = new FragmentNodeProcessService(_mockFragmentNodeProcessRepository.Object);
            _fragmentNodeFragmentService = new FragmentNodeFragmentService(_mockFragmentNodeFragmentRepository.Object);
            _flowFlowPropertyService = new FlowFlowPropertyService(_mockFlowFlowPropertyRepository.Object);
            _dependencyParamService = new DependencyParamService(_mockDependencyParamRepository.Object);
            _flowPropertyParamService = new FlowPropertyParamService(_mockFlowPropertyParamRepository.Object);
            _flowPropertyParamService = new FlowPropertyParamService(_mockFlowPropertyParamRepository.Object);
            _paramService = new ParamService(_mockParamRepository.Object);
            _scoreCacheService = new ScoreCacheService(_mockScoreCacheRepository.Object);
            _processSubstitutionService = new ProcessSubstitutionService(_mockProcessSubstitutionRepository.Object);
            _fragmentSubstitutionService = new FragmentSubstitutionService(_mockFragmentSubstitutionRepository.Object);
            // _scenarioBackgroundService = new ScenarioBackgroundService(_mockScenarioBackgroundRepository.Object);
            _backgroundService = new BackgroundService(_mockBackgroundRepository.Object);
            _processEmissionParamService = new ProcessEmissionParamService(_mockProcessEmissionParamRepository.Object);
            _flowPropertyEmissionService = new FlowPropertyEmissionService(_mockFlowPropertyEmissionRepository.Object);
            _processDissipationService = new ProcessDissipationService(_mockProcessDissipationRepository.Object);
            _processDissipationParamService = new ProcessDissipationParamService(_mockProcessDissipationParamRepository.Object);
            _lciaService = new LCIAService(_mockLCIARepository.Object);
            _characterizationParamService = new CharacterizationParamService(_mockCharacterizationParamRepository.Object);
            _unitOfWork = _mockUnitOfWork.Object;

            

        //private IUnitOfWork _unitOfWork;

            _fragmentTraversalV2 = new FragmentTraversalV2(//_flowService,
            _fragmentFlowService,
            _nodeCacheService,
            //_fragmentNodeProcessService,
            _processFlowService,
            //_fragmentNodeFragmentService,
            _flowFlowPropertyService,
            _dependencyParamService,
            //_flowPropertyParamService,
            //_fragmentService,
            //_paramService,
            _unitOfWork);

            _fragmentLCIAComputation = new FragmentLCIAComputation(_fragmentFlowService,
            _scoreCacheService,
            _nodeCacheService,
            _fragmentNodeProcessService,
            _processSubstitutionService,
            _fragmentNodeFragmentService,
            _fragmentSubstitutionService,
            _lciaMethodService,
            // _scenarioBackgroundService,
            _backgroundService,
            _processFlowService,
            _processEmissionParamService,
            _flowService,
            _flowFlowPropertyService,
            _flowPropertyParamService,
            _flowPropertyEmissionService,
            _processDissipationService,
            _processDissipationParamService,
            _lciaService,
            _characterizationParamService,
            _paramService,
            _dependencyParamService,
            _fragmentService,
            _processService,
            _unitOfWork);

            _lciaComputation = new LCIAComputationV2(_processFlowService,
            _processEmissionParamService,
            _lciaMethodService,
            _flowService,
            _flowFlowPropertyService,
            _flowPropertyParamService,
            _flowPropertyEmissionService,
            _processDissipationService,
            _processDissipationParamService,
            _lciaService,
            _characterizationParamService,
            _paramService);

            _resourceServiceFacade = new ResourceServiceFacade(_categoryService,
                               _classificationService,
                               _fragmentService,
                               _fragmentFlowService,
                               _fragmentTraversalV2,
                               _fragmentLCIAComputation,
                               _flowService,
                               _flowPropertyService,
                               _flowTypeService,
                               _impactCategoryService,
                               _lciaComputation,
                               _lciaMethodService,
                               _processService,
                               _processFlowService,
                               _scenarioService);

            _resourceController = new ResourceController(_resourceServiceFacade);

            //Act
            List<FlowTypeResource> result = _resourceController.GetFlowTypes().ToList();
            //Assert
            //Assert.AreEqual(_flowTypeResources, result);
            //CollectionAssert.AreEqual(_flowTypeResources, result);
           Assert.AreEqual(_flowTypeResources.Count,result.Count);
           //Assert.AreEqual(_flowTypeResources.Last(), result.Last());
        }


    }

     
}
