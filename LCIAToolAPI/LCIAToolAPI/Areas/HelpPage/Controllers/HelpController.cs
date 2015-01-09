using System;
using System.Web.Http;
using System.Web.Mvc;
using LCAToolAPI.Areas.HelpPage.ModelDescriptions;
using LCAToolAPI.Areas.HelpPage.Models;

namespace LCAToolAPI.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";

        /// <summary>
        /// Constructor using generic configuration.
        /// </summary>
        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }

        /// <summary>
        /// Constructor using specified configuration
        /// </summary>
        /// <param name="config">an HttpConfiguration object</param>
        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        /// <summary>
        /// The HttpConfiguration object for the controller instance.
        /// </summary>
        public HttpConfiguration Configuration { get; private set; }

        /// <summary>
        /// Action to perform when loading the index page.  Returns the ApiExplorer View.
        /// </summary>
        /// <returns>an ActionResult</returns>
        public ActionResult Index()
        {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        /// <summary>
        /// Action to perform when loading the help page for a specific API route.
        /// </summary>
        /// <param name="apiId">the API route descriptor</param>
        /// <returns>an ActionResult</returns>
        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        /// <summary>
        /// Action to perform when loading the help page for a resource model.
        /// </summary>
        /// <param name="modelName">the resource descriptor</param>
        /// <returns>an ActionResult</returns>
        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}