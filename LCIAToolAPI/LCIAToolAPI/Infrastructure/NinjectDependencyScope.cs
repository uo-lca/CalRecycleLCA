using System;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;
using Ninject.Activation;
using Ninject.Parameters;
using System.Collections.Generic;
using System.Linq;

namespace LCAToolAPI.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class NinjectDependencyScope : IDependencyScope
    {
        /// <summary>
        /// 
        /// </summary>
         protected IResolutionRoot resolutionRoot;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
         public NinjectDependencyScope(IResolutionRoot kernel)
        {
            resolutionRoot = kernel;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            IRequest request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return resolutionRoot.Resolve(request).SingleOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            IRequest request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return resolutionRoot.Resolve(request).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            IDisposable disposable = (IDisposable)resolutionRoot;
            if (disposable != null) disposable.Dispose();
            resolutionRoot = null;
        }
    }
}