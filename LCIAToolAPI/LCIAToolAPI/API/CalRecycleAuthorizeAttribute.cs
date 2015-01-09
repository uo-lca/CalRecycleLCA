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
    /// <summary>
    /// Custom authorization scheme.  The principle is that CalRecycle's server determines the ScenarioGroup membership
    /// of the user based on their calrecycle.ca.gov login, and instructs the user's client to pass us a token.  
    /// Token is used to perform auth.
    /// 
    /// Since the authorization has to happen prior to dependency injection, all the attribute can do is read the results 
    /// of the authentication request and write them into the actionContext for the controller to read.  The service layer 
    /// then determines which scenarios are accessible based on group membership.
    /// 
    /// Currently the API does support editing the base scenarios as long as the request is authenticated as 
    /// ScenarioGroupID 1.  Depending on the security of the implementation, this may need to be more restricted
    /// (e.g. through CORS)
    /// 
    /// Extends AuthorizeAttribute
    /// </summary>
    public class CalRecycleAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// overrides inherited action to take upon authorization
        /// </summary>
        /// <param name="actionContext">an HttpActionContext </param>
        public override void OnAuthorization( HttpActionContext actionContext)
        {
            string authString = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query)
                .Get("auth");

            KeyValuePair<string, object> authData = new KeyValuePair<string,object> ( "authString", authString);

            actionContext.ControllerContext.RouteData.Values.Add(authData);
                
        }

        /// <summary>
        /// All connections are authorized if the scenarioId belongs to group 1; but that cannot be known at this time
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return true;
        }
            
            
    }
}