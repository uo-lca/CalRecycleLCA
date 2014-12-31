using System;
using Ninject;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http.Headers;
using CalRecycleLCA.Services;

namespace LCAToolAPI.API
{

    public class CalRecycleAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization( HttpActionContext actionContext)
        {
            string authString = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query)
                .Get("auth");

            KeyValuePair<string, object> authData = new KeyValuePair<string,object> ( "authString", authString);

            actionContext.ControllerContext.RouteData.Values.Add(authData);
                
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return true;
        }
            
            
    }
}