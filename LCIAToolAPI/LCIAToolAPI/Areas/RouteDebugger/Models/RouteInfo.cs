using System.Collections.Generic;

namespace LCIAToolAPI.Areas.RouteDebugger.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string RouteTemplate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public KeyValuePair<string, string>[] Defaults { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public KeyValuePair<string, string>[] Constraints { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public KeyValuePair<string, string>[] DataTokens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Handler { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Picked { get; set; }
    }
}
