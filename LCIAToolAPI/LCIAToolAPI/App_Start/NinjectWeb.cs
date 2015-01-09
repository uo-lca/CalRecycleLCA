[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(LCAToolAPI.App_Start.NinjectWeb), "Start")]

namespace LCAToolAPI.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject.Web;

    /// <summary>
    /// 
    /// </summary>
    public static class NinjectWeb 
    {
        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
        }
    }
}
