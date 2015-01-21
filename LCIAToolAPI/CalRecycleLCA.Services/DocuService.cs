using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        private string UrlRoot(HttpRequestContext request)
        {
            return MyTrimEnd(request.Url.Request.RequestUri.AbsoluteUri, request.Url.Request.RequestUri.AbsolutePath)
                + "/";
            //return new Uri(request.Url.Request.RequestUri, UriKind.Relative).ToString();
        }

        private string ServicesVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public ApiInfo ApiInfo(HttpRequestContext request)
        {
            var urlRoot = UrlRoot(request);
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
                    new Link() {
                        Rel = "help",
                        Title = "CalRecycle LCA Data API Documentation",
                        Href = urlRoot + "Help"
                    },
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
    }
}
