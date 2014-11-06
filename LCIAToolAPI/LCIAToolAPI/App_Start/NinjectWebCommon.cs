[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(LCAToolAPI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(LCAToolAPI.App_Start.NinjectWebCommon), "Stop")]

namespace LCAToolAPI.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using CalRecycleLCA.Services;
    using LCAToolAPI.Infrastructure;
    using System.Web.Http;
    using LcaDataModel;
    using Repository.Pattern.UnitOfWork;
	using Repository.Pattern.Repositories;
	using Repository.Pattern.Ef6;
	using Service.Pattern;
	using Repository.Pattern.DataContext;
	using Repository.Pattern.Ef6.Factories;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();
        public static IKernel Kernel
        {
            get
            {
                return bootstrapper.Kernel;
            }
        }

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>));
            kernel.Bind(typeof(IRepository<Param>)).To(typeof(Repository<Param>));
            kernel.Bind<IDataContext>().To<UsedOilLCAContext>();
            kernel.Bind<IDataContextAsync>().To<UsedOilLCAContext>();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IUnitOfWorkAsync>().To<UnitOfWork>();
            kernel.Bind<IFragmentTraversalV2>().To<FragmentTraversalV2>();
            //kernel.Bind<IFlowPropertyParamService>().To<FlowPropertyParamService>();
            kernel.Bind<ILCIAComputationV2>().To<LCIAComputationV2>();
            kernel.Bind<IFragmentLCIAComputation>().To<FragmentLCIAComputation>();
            kernel.Bind<IResourceServiceFacade>().To<ResourceServiceFacade>();

            //kernel.Bind(typeof(IService<>)).To(typeof(Service<>));

            kernel.Bind(typeof(IRepositoryAsync<Category>)).To(typeof(Repository<Category>));
            kernel.Bind(typeof(IRepositoryAsync<Classification>)).To(typeof(Repository<Classification>));
            kernel.Bind(typeof(IRepositoryAsync<Fragment>)).To(typeof(Repository<Fragment>));
            kernel.Bind(typeof(IRepositoryAsync<FragmentFlow>)).To(typeof(Repository<FragmentFlow>));
            kernel.Bind(typeof(IRepositoryAsync<Flow>)).To(typeof(Repository<Flow>));
            kernel.Bind(typeof(IRepositoryAsync<FlowProperty>)).To(typeof(Repository<FlowProperty>));
            kernel.Bind(typeof(IRepositoryAsync<FlowType>)).To(typeof(Repository<FlowType>));
            kernel.Bind(typeof(IRepositoryAsync<ImpactCategory>)).To(typeof(Repository<ImpactCategory>));
            kernel.Bind(typeof(IRepositoryAsync<LCIAMethod>)).To(typeof(Repository<LCIAMethod>));
            kernel.Bind(typeof(IRepositoryAsync<Process>)).To(typeof(Repository<Process>));
            kernel.Bind(typeof(IRepositoryAsync<ProcessFlow>)).To(typeof(Repository<ProcessFlow>));
            kernel.Bind(typeof(IRepositoryAsync<Scenario>)).To(typeof(Repository<Scenario>));
            kernel.Bind(typeof(IRepositoryAsync<Param>)).To(typeof(Repository<Param>));
            kernel.Bind(typeof(IRepositoryAsync<NodeCache>)).To(typeof(Repository<NodeCache>));
            kernel.Bind(typeof(IRepositoryAsync<FragmentNodeProcess>)).To(typeof(Repository<FragmentNodeProcess>));
            kernel.Bind(typeof(IRepositoryAsync<FragmentNodeFragment>)).To(typeof(Repository<FragmentNodeFragment>));
            kernel.Bind(typeof(IRepositoryAsync<FlowFlowProperty>)).To(typeof(Repository<FlowFlowProperty>));
            kernel.Bind(typeof(IRepositoryAsync<DependencyParam>)).To(typeof(Repository<DependencyParam>));
            kernel.Bind(typeof(IRepositoryAsync<FlowPropertyParam>)).To(typeof(Repository<FlowPropertyParam>));
            kernel.Bind(typeof(IRepositoryAsync<ProcessEmissionParam>)).To(typeof(Repository<ProcessEmissionParam>));
            kernel.Bind(typeof(IRepositoryAsync<FlowPropertyEmission>)).To(typeof(Repository<FlowPropertyEmission>));
            kernel.Bind(typeof(IRepositoryAsync<ProcessDissipation>)).To(typeof(Repository<ProcessDissipation>));
            kernel.Bind(typeof(IRepositoryAsync<ProcessDissipationParam>)).To(typeof(Repository<ProcessDissipationParam>));
            kernel.Bind(typeof(IRepositoryAsync<CharacterizationParam>)).To(typeof(Repository<CharacterizationParam>));
            kernel.Bind(typeof(IRepositoryAsync<LCIA>)).To(typeof(Repository<LCIA>));
            kernel.Bind(typeof(IRepositoryAsync<ScoreCache>)).To(typeof(Repository<ScoreCache>));
            kernel.Bind(typeof(IRepositoryAsync<ProcessSubstitution>)).To(typeof(Repository<ProcessSubstitution>));
            kernel.Bind(typeof(IRepositoryAsync<FragmentSubstitution>)).To(typeof(Repository<FragmentSubstitution>));
            kernel.Bind(typeof(IRepositoryAsync<BackgroundSubstitution>)).To(typeof(Repository<BackgroundSubstitution>));
            kernel.Bind(typeof(IRepositoryAsync<Background>)).To(typeof(Repository<Background>));


            kernel.Bind<IRepositoryProvider>().To<RepositoryProvider>();
            kernel.Bind<ICategoryService>().To<CategoryService>();
            kernel.Bind<IClassificationService>().To<ClassificationService>();
            kernel.Bind<IFragmentService>().To<FragmentService>();
            kernel.Bind<IFragmentFlowService>().To<FragmentFlowService>();
            kernel.Bind<IFlowService>().To<FlowService>();
            kernel.Bind<IFlowPropertyService>().To<FlowPropertyService>();
            kernel.Bind<IFlowTypeService>().To<FlowTypeService>();
            kernel.Bind<IImpactCategoryService>().To<ImpactCategoryService>();
            kernel.Bind<ILCIAMethodService>().To<LCIAMethodService>();
            kernel.Bind<IProcessService>().To<ProcessService>();
            kernel.Bind<IProcessFlowService>().To<ProcessFlowService>();
            kernel.Bind<IScenarioService>().To<ScenarioService>();
            kernel.Bind(typeof(INodeCacheService)).To(typeof(NodeCacheService));
            kernel.Bind(typeof(IFragmentNodeProcessService)).To(typeof(FragmentNodeProcessService));
            kernel.Bind(typeof(IFragmentNodeFragmentService)).To(typeof(FragmentNodeFragmentService));
            kernel.Bind(typeof(IFlowFlowPropertyService)).To(typeof(FlowFlowPropertyService));
            kernel.Bind(typeof(IProcessEmissionParamService)).To(typeof(ProcessEmissionParamService));
            kernel.Bind(typeof(IFlowPropertyEmissionService)).To(typeof(FlowPropertyEmissionService));
            kernel.Bind(typeof(IProcessDissipationService)).To(typeof(ProcessDissipationService));
            kernel.Bind(typeof(IProcessDissipationParamService)).To(typeof(ProcessDissipationParamService));
            kernel.Bind(typeof(ILCIAService)).To(typeof(LCIAService));
            kernel.Bind(typeof(ICharacterizationParamService)).To(typeof(CharacterizationParamService));
            kernel.Bind(typeof(IParamService)).To(typeof(ParamService));
            kernel.Bind(typeof(IScoreCacheService)).To(typeof(ScoreCacheService));
            kernel.Bind(typeof(IProcessSubstitutionService)).To(typeof(ProcessSubstitutionService));
            kernel.Bind(typeof(IFragmentSubstitutionService)).To(typeof(FragmentSubstitutionService));
            // kernel.Bind(typeof(IScenarioBackgroundService)).To(typeof(ScenarioBackgroundService));
            kernel.Bind(typeof(IBackgroundService)).To(typeof(BackgroundService));
            kernel.Bind<IDependencyParamService>().To<DependencyParamService>();
            kernel.Bind<IFlowPropertyParamService>().To<FlowPropertyParamService>();





          


            //kernel.Bind(typeof(IService<LCIAMethod>)).To(typeof(Service<LCIAMethod>));
            //kernel.Bind(typeof(IService<Process>)).To(typeof(Service<Process>));
            //kernel.Bind(typeof(IService<Flow>)).To(typeof(Service<Flow>));
            //kernel.Bind(typeof(IService<FragmentFlow>)).To(typeof(Service<FragmentFlow>));
            //kernel.Bind(typeof(IService<NodeCache>)).To(typeof(Service<NodeCache>));
            //kernel.Bind(typeof(IService<FragmentNodeProcess>)).To(typeof(Service<FragmentNodeProcess>));
            //kernel.Bind(typeof(IService<ProcessFlow>)).To(typeof(Service<ProcessFlow>));
            //kernel.Bind(typeof(IService<FragmentNodeFragment>)).To(typeof(Service<FragmentNodeFragment>));
            //kernel.Bind(typeof(IService<FlowFlowProperty>)).To(typeof(Service<FlowFlowProperty>));
            //kernel.Bind(typeof(IService<DependencyParam>)).To(typeof(Service<DependencyParam>));
            //kernel.Bind(typeof(IService<Param>)).To(typeof(Service<Param>));
            //kernel.Bind(typeof(IService<FlowPropertyParam>)).To(typeof(Service<FlowPropertyParam>));
            //kernel.Bind(typeof(IService<Fragment>)).To(typeof(Service<Fragment>));
            //kernel.Bind(typeof(IService<ProcessEmissionParam>)).To(typeof(Service<ProcessEmissionParam>));
            //kernel.Bind(typeof(IService<FlowPropertyEmission>)).To(typeof(Service<FlowPropertyEmission>));
            //kernel.Bind(typeof(IService<ProcessDissipation>)).To(typeof(Service<ProcessDissipation>));
            //kernel.Bind(typeof(IService<ProcessDissipationParam>)).To(typeof(Service<ProcessDissipationParam>));
            //kernel.Bind(typeof(IService<LCIA>)).To(typeof(Service<LCIA>));
            //kernel.Bind(typeof(IService<CharacterizationParam>)).To(typeof(Service<CharacterizationParam>));
            //kernel.Bind(typeof(IService<Param>)).To(typeof(Service<Param>));
        
        
        //[Inject]
        //private readonly IService<Param> _paramService;

            kernel.Bind<ITestGenericService>().To<TestGenericService>();
        }

        public static void RegisterNinject(HttpConfiguration configuration)
        {
            // Set Web API Resolver
            configuration.DependencyResolver = new NinjectDependencyResolver(bootstrapper.Kernel);

        }
    }
}
