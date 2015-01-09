using System.Net;
using System.Net.Http;

namespace LCIAToolAPI.Areas.RouteDebugger.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class InspectData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public InspectData(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(RequestHelper.ActionCache))
            {
                Action = request.Properties[RequestHelper.ActionCache] as ActionSelectionLog;
            }

            if (request.Properties.ContainsKey(RequestHelper.ControllerCache))
            {
                Controller = request.Properties[RequestHelper.ControllerCache] as ControllerSelectionInfo[];
            }

            if (request.Properties.ContainsKey(RequestHelper.RoutesCache))
            {
                Routes = request.Properties[RequestHelper.RoutesCache] as RouteInfo[];
            }

            if (request.Properties.ContainsKey(RequestHelper.RouteDataCache))
            {
                RouteData = request.Properties[RequestHelper.RouteDataCache] as RouteDataInfo;
            }

            if (request.Properties.ContainsKey(RequestHelper.SelectedController))
            {
                SelectedController = request.Properties[RequestHelper.SelectedController] as string;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ActionSelectionLog Action { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ControllerSelectionInfo[] Controller { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RouteInfo[] Routes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RouteDataInfo RouteData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HttpStatusCode RealHttpStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SelectedController { get; set; }
    }
}
