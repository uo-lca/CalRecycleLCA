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
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = 
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                                                                                                                
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;


            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = //Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = // Newtonsoft.Json.PreserveReferencesHandling.Objects;
                                                                                                                       Newtonsoft.Json.PreserveReferencesHandling.None;
            // Routes are defined in Controller classes

            config.MapHttpAttributeRoutes();

            
        }
    }
}