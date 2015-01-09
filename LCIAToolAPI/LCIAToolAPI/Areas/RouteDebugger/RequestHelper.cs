using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Hosting;
using LCIAToolAPI.Areas.RouteDebugger.Components;

namespace LCIAToolAPI.Areas.RouteDebugger
{
    /// <summary>
    /// 
    /// </summary>
    public static class RequestHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string InspectHeaderName = "RouteInspecting";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string RouteDataCache = "RD_ROUTEDATA";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string RoutesCache = "RD_ROUTES";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string ControllerCache = "RD_CONTROLLER";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string ActionCache = "RD_ACTION";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string SelectedController = "RD_SELECTED_CONTROLLER";

        /// <summary>
        /// Returns true if this request is a inspect request. 
        /// 
        /// For sake of security only inspect request from local will be accepted.
        /// </summary>
        public static bool IsInspectRequest(this HttpRequestMessage request)
        {
            IEnumerable<string> values;

            if (request.Headers.TryGetValues(InspectHeaderName, out values))
            {
                if (String.Equals(values.FirstOrDefault(), "true", StringComparison.InvariantCulture))
                {
                    return request.IsFromLocal();
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if this request is from local
        /// </summary>
        public static bool IsFromLocal(this HttpRequestMessage request)
        {
            if (request == null)
            {
                return false;
            }

            Lazy<bool> isLocal;
            if (request.Properties.TryGetValue<Lazy<bool>>(HttpPropertyKeys.IsLocalKey, out isLocal))
            {
                return isLocal.Value;
            }

            return false;
        }
    }
}
