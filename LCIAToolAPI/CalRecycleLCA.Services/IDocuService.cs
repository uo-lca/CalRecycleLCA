using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Entities.Models;


namespace CalRecycleLCA.Services
{
    public interface IDocuService
    {
        string MyTrimEnd(string source, string value);
        string UrlRoot(HttpRequestMessage request);
        ApiInfo ApiInfo(HttpRequestContext request);
        Version ApiVersion();
        List<Link> ResourceLinks(HttpActionContext action, Resource resource);
    }
}
