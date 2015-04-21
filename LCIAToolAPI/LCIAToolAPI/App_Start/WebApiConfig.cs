using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Repository;
using LcaDataModel;
using Newtonsoft.Json.Converters;

namespace LCIAToolAPI.App_Start
{
    /// <summary>
    /// Main config for Web API controller
    /// </summary>
    public class WebApiConfig
    {
        /// <summary>
        /// register Web API controller
        /// </summary>
        /// <param name="config">HttpConfiguration </param>
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();

            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            //config.Formatters.Remove(config.Formatters.XmlFormatter);
            //config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.JsonFormatter.MediaTypeMappings
                .Add(new RequestHeaderMapping("Accept", "text/html", StringComparison.InvariantCultureIgnoreCase, true, "application/json"));

            //config.Formatters.JsonFormatter.SupportedMediaTypes.Remove(new MediaTypeHeaderValue("text/xml"));
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings();
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = 
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            //GlobalConfiguration.Configuration
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = //Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = // Newtonsoft.Json.PreserveReferencesHandling.Objects;
                Newtonsoft.Json.PreserveReferencesHandling.None;
            // Routes are defined in Controller classes

            config.MapHttpAttributeRoutes();

            
        }
    }
}