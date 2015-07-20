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

        public string MyTrimEnd(string source, string value)
        {
            if (!source.EndsWith(value))
                return source;

            return source.Remove(source.LastIndexOf(value));
        }

        public string UrlRoot(HttpRequestMessage request)
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

        private Link SelfLink(string selfUrl, string title)
        {
            return new Link()
            {
                Rel = "self",
                Title = title,
                Href = selfUrl
            };
        }

        private List<Link> AddFlowLinks(string selfUrl, string title)
        {
            return new List<Link>() {
                SelfLink(selfUrl, title),
                new Link() {
                    Rel = "flow properties",
                    Title = "Flow Property magnitudes for Flow",
                    Href = selfUrl + "/flowpropertymagnitudes"
                }
            };
        }

        private List<Link> AddProcessLinks(string selfUrl, string title, bool? isPrivate)
        {
            var links = new List<Link>() { 
                SelfLink(selfUrl, title)
            };
            if (!(bool)isPrivate)
                links.Add(new Link() {
                    Rel = "process flows",
                    Title = "Exchanges for Process",
                    Href = selfUrl + "/processflows"
                });
            links.Add(new Link() {
                Rel = "process dissipation",
                Title = "Environmental dissipation of flow constituents by the process",
                Href = selfUrl + "/dissipation"
            });
            links.AddRange(new List<Link>() {
                new Link() {
                    Rel = "flow properties",
                    Title = "Flow Properties belonging to flows exchanged by the process",
                    Href = selfUrl + "/flowproperties"
                },
                new Link() {
                    Rel = "cumulative scores",
                    Title = "Cumulative LCIA results for all LCIA methods",
                    Href = selfUrl + "/lciaresults"
                }
            });
            if (!(bool)isPrivate)
                links.Add(new Link()
                {
                    Rel = "detailed scores",
                    Title = "Detailed LCIA results for a single LCIA method",
                    Href = selfUrl + "/lciamethods/{x}/lciaresults"
                });
            return links;
        }

        private List<Link> AddFlowPropertyLinks(string selfUrl, string title)
        {
            return new List<Link>() { 
                SelfLink(selfUrl, title)
            };
        }
        private List<Link> AddLCIAMethodLinks(string selfUrl, string title)
        {
            return new List<Link>() {
                SelfLink(selfUrl, title),
                new Link() {
                    Rel = "flows",
                    Title = "Flows present in the database characterized under this method",
                    Href = selfUrl + "/flows"
                },
                new Link() {
                    Rel = "factors",
                    Title = "Characterization factors for the LCIA method (only for flows present in the database)",
                    Href = selfUrl + "/factors"
                }
            };
        }
        private List<Link> AddFragmentLinks(string selfUrl, string title)
        {
            return new List<Link>() {
                SelfLink(selfUrl, title),
                new Link() {
                    Rel = "fragment flows",
                    Title = "Dependency relationships modeled by this fragment, and traversal results",
                    Href = selfUrl + "/fragmentflows"
                },
                new Link() {
                    Rel = "flows",
                    Title = "Flows present in the fragment",
                    Href = selfUrl + "/flows"
                },
                new Link() {
                    Rel = "flow properties",
                    Title = "Flow Properties for Flows present in the fragment",
                    Href = selfUrl + "/flowproperties"
                },
                new Link() {
                    Rel = "fragment stages",
                    Title = "Aggregation Stages encountered during recursive traversal of this fragment",
                    Href = selfUrl + "/stages"
                },
                new Link() {
                    Rel = "flat stages",
                    Title = "Aggregation Stages belonging to this fragment (and not sub-fragments)",
                    Href = selfUrl + "/f/stages"
                },
                new Link() {
                    Rel = "cumulative scores",
                    Title = "LCIA results for the fragment, aggregated recursively by stage",
                    Href = selfUrl + "/lciaresults"
                },
                new Link() {
                    Rel = "flat cumulative scores",
                    Title = "LCIA results for the fragment, aggregated by flat stages",
                    Href = selfUrl + "/f/lciaresults"
                },
                new Link() {
                    Rel = "sensitivity",
                    Title = "LCIA sensitivity to a parameter; must be authorized; see docs",
                    Href = selfUrl + "/params/{x}/sensitivity"
                },
                new Link() {
                    Rel = "sensitivity",
                    Title = "LCIA sensitivity to an ad hoc parameter specified in URI params; see docs",
                    Href = selfUrl + "/sensitivity?id=..."
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
            var selfUrl = MyTrimEnd(action.Request.RequestUri.AbsoluteUri,"/");
            List<Link> links = new List<Link>();
            if (resource.ResourceType != "Fragment")
                links.Add(XmlLink(urlRoot, resource.UUID, resource.Version));
            switch (resource.ResourceType)
            {
                    // for each case, we need to (1) remove filters if present and (2) add instance if absent
                case "Flow":
                    {
                        if (action.ActionArguments.ContainsKey("flowTypeId"))
                            selfUrl = urlRoot + "api/flows";
                        if (!action.ActionArguments.ContainsKey("flowId"))
                        {
                            selfUrl += "/" + resource.ID;
                            links.Add(SelfLink(selfUrl, resource.Name));
                        }
                        else // only provide detailed links for single references
                            links.AddRange(AddFlowLinks(selfUrl, resource.Name));
                        break;
                    }
                case "Process":
                    {
                        if (action.ActionArguments.ContainsKey("flowTypeId"))
                            selfUrl = urlRoot + "api/processes";
                        if (!action.ActionArguments.ContainsKey("processId"))
                        {
                            selfUrl += "/" + resource.ID;
                            links.Add(SelfLink(selfUrl, resource.Name));
                        }
                        else
                            links.AddRange(AddProcessLinks(selfUrl, resource.Name, resource.isPrivate));
                        break;
                    }
                case "FlowProperty":
                    {
                        if (!action.ActionArguments.ContainsKey("flowPropertyId"))
                        {
                            selfUrl += "/" + resource.ID;
                            links.Add(SelfLink(selfUrl, resource.Name));
                        }
                        else
                            links.AddRange(AddFlowPropertyLinks(selfUrl, resource.Name));
                        break;
                    }
                case "LCIAMethod":
                    {
                        if (action.ActionArguments.ContainsKey("impactCategoryId"))
                            selfUrl = urlRoot + "api/lciamethods";
                        if (!action.ActionArguments.ContainsKey("lciaMethodId"))
                        {
                            selfUrl += "/" + resource.ID;
                            links.Add(SelfLink(selfUrl, resource.Name));
                        }
                        else
                            links.AddRange(AddLCIAMethodLinks(selfUrl, resource.Name));
                        break;
                    }
                case "Fragment":
                    {
                        if (!action.ActionArguments.ContainsKey("fragmentId"))
                        {
                            selfUrl += "/" + resource.ID;
                            links.Add(SelfLink(selfUrl, resource.Name));
                        }
                        else
                            links.AddRange(AddFragmentLinks(selfUrl, resource.Name));
                        break;
                    }
            }

            return links;
        }
    }
}
