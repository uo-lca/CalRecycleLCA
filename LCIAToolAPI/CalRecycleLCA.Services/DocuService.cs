using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    /// <summary>
    /// Provides documentation and links information to the API
    /// </summary>
    public class DocuService : IDocuService
    {

        private string MyTrimEnd(string source, string value)
        {
            if (!source.EndsWith(value))
                return source;

            return source.Remove(source.LastIndexOf(value));
        }

        private string UrlRoot(HttpRequestMessage request)
        {
            return MyTrimEnd(request.RequestUri.AbsoluteUri, request.RequestUri.PathAndQuery)
                + MyTrimEnd(request.GetRequestContext().VirtualPathRoot,"/")
                + "/";
            //return new Uri(request.Url.Request.RequestUri, UriKind.Relative).ToString();
        }

        private string ServicesVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private Link HelpLink(string urlRoot)
        {
            return new Link()
            {
                Rel = "help",
                Title = "CalRecycle LCA Data API Documentation",
                Href = urlRoot + "Help"
            };
        }

        private Link XmlLink(string urlRoot, string uuid, string version="")
        {
            string href = urlRoot + "xml/" + uuid;
            if (! String.IsNullOrEmpty(version))
                href += "?version=" + version;
            return new Link()
            {
                Rel = "reference",
                Title = "Link to ILCD Data Set",
                Href = href
            };
        }

        private List<Link> AddFlowLinks(string selfUrl)
        {
            return new List<Link>() {
                new Link() {
                    Rel = "self",
                    Title = "Flow",
                    Href = selfUrl
                },
                new Link() {
                    Rel = "flow properties",
                    Title = "Flow Properties for Flow",
                    Href = selfUrl + "/flowproperties"
                }
            };
        }

        public ApiInfo ApiInfo(HttpRequestContext request)
        {
            var k = request.Url.Request;

            var urlRoot = UrlRoot(request.Url.Request);
            return new ApiInfo
            {
                Title = "CalRecycle Used Oil LCA API",
                Maintainer = new Contact
                {
                    Name = "Brandon Kuczenski",
                    Email = "bkuczenski@ucsb.edu"
                },
                Version = ServicesVersion(),
                Links = new List<Link>
                {
                    HelpLink(urlRoot),
                    new Link () {
                        Rel = "license",
                        Title = "BSD License",
                        Href = urlRoot + "LICENSE"
                    }
                }
            };
        }   
        public Version ApiVersion()
        {
            return new Version(ServicesVersion());
        }

        public List<Link> ResourceLinks(HttpActionContext action, Resource resource)
        {
            var urlRoot = UrlRoot(action.Request);
            var selfUrl = action.Request.RequestUri.AbsoluteUri;
            List<Link> links = new List<Link>();
            links.Add(XmlLink(urlRoot, resource.UUID, resource.Version));
            switch (resource.ResourceType)
            {
                case "Flow":
                    {
                        if (action.ActionArguments.ContainsKey("flowTypeId"))
                            selfUrl = urlRoot + "api/flows";
                        if (! action.ActionArguments.ContainsKey("flowId"))
                            selfUrl += "/" + resource.ID;
                        links.AddRange(AddFlowLinks(selfUrl));
                        break;
                    }
            }

            return links;
        }
    }
}
