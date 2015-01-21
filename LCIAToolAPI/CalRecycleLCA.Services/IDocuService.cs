using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Entities.Models;


namespace CalRecycleLCA.Services
{
    public interface IDocuService
    {
        ApiInfo ApiInfo(HttpRequestContext request);
        Version ApiVersion();
    }
}
