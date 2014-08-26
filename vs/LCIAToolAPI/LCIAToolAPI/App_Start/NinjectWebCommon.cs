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
            kernel.Bind<IFragmentService>().To<FragmentService>();
            kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>));
            kernel.Bind(typeof(IRepository<Param>)).To(typeof(Repository<Param>));
            kernel.Bind<IDbContext>().To<UsedOilLCAContext>();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IFragmentFlowService>().To<FragmentFlowService>();
            kernel.Bind<IDependencyParamService>().To<DependencyParamService>();
            kernel.Bind<IFragmentTraversalV2>().To<FragmentTraversalV2>();
            kernel.Bind<IFragmentLinkService>().To<FragmentLinkService>();
            kernel.Bind<IParamService>().To<ParamService>();
            kernel.Bind<IProcessService>().To<ProcessService>();
            kernel.Bind<IFlowPropertyService>().To<FlowPropertyService>();
            kernel.Bind<IFlowPropertyParamService>().To<FlowPropertyParamService>();
            kernel.Bind<IFlowFlowPropertyService>().To<FlowFlowPropertyService>();
            kernel.Bind<IFlowService>().To<FlowService>();
            kernel.Bind<INodeCacheService>().To<NodeCacheService>();
            kernel.Bind<IFragmentNodeFragmentService>().To<FragmentNodeFragmentService>();
            kernel.Bind<IFragmentNodeProcessService>().To<FragmentNodeProcessService>();
            kernel.Bind<IProcessFlowService>().To<ProcessFlowService>();
            kernel.Bind<ILCIAComputationV2>().To<LCIAComputationV2>();
            kernel.Bind<IProcessEmissionParamService>().To<ProcessEmissionParamService>();
            kernel.Bind<ILCIAMethodService>().To<LCIAMethodService>();
            kernel.Bind<IFlowPropertyEmissionService>().To<FlowPropertyEmissionService>();
            kernel.Bind<IProcessDissipationService>().To<ProcessDissipationService>();
            kernel.Bind<IProcessDissipationParamService>().To<ProcessDissipationParamService>();
            kernel.Bind<ILCIAService>().To<LCIAService>();
            kernel.Bind<ICharacterizationParamService>().To<CharacterizationParamService>();

            //Trying to inject a generic service to avoid having a service for each model class
            //kernel.Bind(typeof(IService<Flow>)).To(typeof(Service<Flow>))
            //    .When(request => request.Service == typeof(IService<Flow>));

            //kernel.Bind(typeof(IService<>)).To(typeof(Service))
            //    .When(request => request.Service == typeof(IService<FragmentFlow>));
        }

        public static void RegisterNinject(HttpConfiguration configuration)
        {
            // Set Web API Resolver
            configuration.DependencyResolver = new NinjectDependencyResolver(bootstrapper.Kernel);

        }
    }
}
