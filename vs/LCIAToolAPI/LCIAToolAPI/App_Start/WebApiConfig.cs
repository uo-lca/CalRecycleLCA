using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Repository;
using Data;
using Newtonsoft.Json.Converters;
using Services;

namespace LCIAToolAPI.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            var json = config.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            //config.Formatters.Remove(config.Formatters.JsonFormatter);

            //JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            //serializerSettings.ContractResolver = new ExcludeEntityKeyContractResolver();
            //var jsonMediaTypeFormatter = new JsonMediaTypeFormatter();
            //jsonMediaTypeFormatter.SerializerSettings = serializerSettings;
            //GlobalConfiguration.Configuration.Formatters.Insert(0, jsonMediaTypeFormatter);



            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings();
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;


            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

            config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}/{impactCategoryId}",
            defaults: new { id = RouteParameter.Optional, impactCategoryId = RouteParameter.Optional }
            );

        }
    }
}