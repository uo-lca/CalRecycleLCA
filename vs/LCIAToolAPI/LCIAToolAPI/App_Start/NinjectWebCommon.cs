[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(LCAToolAPI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(LCAToolAPI.App_Start.NinjectWebCommon), "Stop")]

namespace LCAToolAPI.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Services;
    using LCAToolAPI.Infrastructure;
    using System.Web.Http;
    using Repository;
    using LcaDataModel;

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
            kernel.Bind<IDbContext>().To<UsedOilLCAContext>();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IDependencyParamService>().To<DependencyParamService>();
            kernel.Bind<IFragmentTraversalV2>().To<FragmentTraversalV2>();
            kernel.Bind<IProcessService>().To<ProcessService>();
            kernel.Bind<IFlowPropertyParamService>().To<FlowPropertyParamService>();
            kernel.Bind<ILCIAComputationV2>().To<LCIAComputationV2>();
            kernel.Bind<IResourceService>().To<ResourceService>();

            kernel.Bind(typeof(IService<>)).To(typeof(Service<>));

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
