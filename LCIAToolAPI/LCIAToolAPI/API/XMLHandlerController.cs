
using Ninject;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Xml;
using LcaDataModel;
using CalRecycleLCA.Services;

namespace LCAToolAPI.API
{
    /// <summary>
    /// this is ultimately supposed to provide access to the XML files (except for processes that are protected).
    /// </summary>
    public class XMLHandlerController : ApiController
    {
        [Inject]
        private readonly IDocuService _docuService;
        [Inject]
        private readonly IILCDEntityService _ilcdEntityService;

        private T verifiedDependency<T>(T dependency) where T : class
        {
            if (dependency == null)
                throw new ArgumentNullException("dependency", String.Format("Type: {0}", dependency.GetType().ToString()));
            else
                return dependency;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowTestService"></param>
        public XMLHandlerController(IDocuService docuService,
            IILCDEntityService ilcdEntityService)
        {
            _docuService = verifiedDependency(docuService);
            _ilcdEntityService = verifiedDependency(ilcdEntityService);
        }

        private HttpResponseMessage CreateXmlResponse(XmlElement content)
        {
            Request.Headers.Accept.Remove(new MediaTypeWithQualityHeaderValue("text/html"));
            var response = Request.CreateResponse(HttpStatusCode.OK, content);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
            return response;
        }

        private Uri CreateRedirect(ILCDEntity entity)
        {
            String uri =(_docuService.UrlRoot(Request) + "xml/" 
                + Enum.GetName(typeof(DataPathEnum),entity.DataTypeID) // DataTypeEnum and DataPathEnum must coincide
                + "/" + entity.UUID + "?version=" + entity.Version);
            return new Uri(uri, UriKind.Absolute);
        }

        private bool TryParse(String uuid, out Guid guid)
        {
            uuid = _docuService.MyTrimEnd(uuid,".xml");
            guid = new Guid();
            if (!Guid.TryParse(uuid, out guid))
                return false;
            return true;
        }



        [Route("xml/{uuid}")]
        [HttpGet]
        public HttpResponseMessage LookupByUUID(String uuid)
        {
            var guid = new Guid();
            if (!TryParse(uuid, out guid))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid UUID.");

            var versionFromQuery = HttpUtility.ParseQueryString(Request.RequestUri.Query)["version"];

            ILCDEntity entity = new ILCDEntity();

            if (String.IsNullOrEmpty(versionFromQuery))
                entity = _ilcdEntityService.LookupUUID(guid.ToString("D")).LastOrDefault();
            else
                entity = _ilcdEntityService.LookupUUID(guid.ToString("D"), versionFromQuery);

            if (entity == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.Found);
                response.Headers.Location = CreateRedirect(entity);
                return response;
            }

        }

        [Route("xml/{dpath}/{uuid}")]
        [HttpGet]
        public HttpResponseMessage LookupByDataTypeAndUUID(String dpath, String uuid)
        {
            var guid = new Guid();
            if (!TryParse(uuid, out guid))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid UUID.");

            if (!Enum.IsDefined(typeof(DataPathEnum),dpath))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid data type.");

            var dataTypeId = Convert.ToInt32(Enum.Parse(typeof(DataPathEnum),dpath));

            var versionFromQuery = HttpUtility.ParseQueryString(Request.RequestUri.Query)["version"];

            ILCDEntity entity = new ILCDEntity();

            if (String.IsNullOrEmpty(versionFromQuery))
                entity = _ilcdEntityService.LookupUUID(guid.ToString("D")).LastOrDefault();
            else
                entity = _ilcdEntityService.LookupUUID(guid.ToString("D"), versionFromQuery);

            if (entity == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            else
            {
                if (dataTypeId != entity.DataTypeID)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, 
                        String.Format("Incorrect data type. Should be {0}",
                        Enum.GetName(typeof(DataPathEnum),(DataPathEnum)entity.DataTypeID)));

                // override default JSON serialization
                Request.Headers.Accept.Remove(new MediaTypeWithQualityHeaderValue("text/html"));
                
                var stringWriter = _ilcdEntityService.GetXmlDocument(entity);

                if (stringWriter == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Source file not found.");

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(stringWriter.ToString(), stringWriter.Encoding, "application/xml");
                return response;
            }

        }


        /*
        /// <summary>
        /// legacy code
        /// </summary>
        /// <returns></returns>
        [Route("api/xmlhandler")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage FlowTest()
        {
            // can I munge the request header?
            Request.Headers.Accept.Remove(new MediaTypeWithQualityHeaderValue("text/html"));
            XmlDocument doc = _flowTestService.ViewXML();

            var stringWriter = new StringWriter();
            doc.Save(stringWriter);

            var content = new StringContent(stringWriter.ToString(), stringWriter.Encoding, "application/xml");

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = content;
            //response.
                
            //    , stringWriter.ToString());
            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
            return response;
        }

        [Route("xml/stylesheets/{filename}")]
        [AcceptVerbs("GET")]
        [HttpGet]
        public HttpResponseMessage GetStylesheet(String filename)
        {
            var stylesheet = _xmlHandler.GetStylesheet(filename);
            if (stylesheet == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            else
                return CreateXmlResponse(stylesheet);
        }
         * */
    }
}
