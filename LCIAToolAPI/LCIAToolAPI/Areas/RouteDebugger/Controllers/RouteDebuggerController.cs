using System.Web.Mvc;

namespace LCIAToolAPI.Areas.RouteDebugger.Controllers
{
    /// <summary>
    /// Just views itself? how does that work?
    /// </summary>
    public class RouteDebuggerController : Controller
    {
        /// <summary>
        /// the action to perform on visiting the index page.  just "View()". wow.
        /// </summary>
        /// <returns>an ActionResult </returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
