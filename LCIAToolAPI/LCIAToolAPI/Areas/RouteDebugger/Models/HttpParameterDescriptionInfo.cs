using System;
using System.Web.Http.Controllers;

namespace LCIAToolAPI.Areas.RouteDebugger.Models
{
    /// <summary>
    /// Represents the parameters.
    /// </summary>
    public class HttpParameterDescriptorInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        public HttpParameterDescriptorInfo(HttpParameterDescriptor descriptor)
        {
            ParameterName = descriptor.ParameterName;
            ParameterType = descriptor.ParameterType;
            ParameterTypeName = descriptor.ParameterType.Name;
        }
        /// <summary>
        /// 
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type ParameterType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParameterTypeName { get; set; }
    }
}
