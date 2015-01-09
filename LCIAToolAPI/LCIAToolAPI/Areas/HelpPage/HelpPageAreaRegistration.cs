using System.Web.Http;
using System.Web.Mvc;

namespace LCAToolAPI.Areas.HelpPage
{
    /// <summary>
    /// registers the area with the VS / .NET / IIS beast
    /// </summary>
    public class HelpPageAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// Assert the name to be "HelpPage"
        /// </summary>
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        /// <summary>
        /// the action taken to register the area
        /// </summary>
        /// <param name="context">an AreaRegistrationContext </param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HelpPage_Default",
                "Help/{action}/{apiId}",
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });

            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}