using System.Collections.Generic;

namespace LCIAToolAPI.Areas.RouteDebugger.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteDataInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string RouteTemplate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public KeyValuePair<string, string>[] Data { get; set; }
    }
}
