
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
        [Inject]
        private readonly IProcessService _processService;
        [Inject]
        private readonly IFlowService _flowService;
        [Inject]
        private readonly IFlowPropertyService _flowPropertyService;
        [Inject]
        private readonly ILCIAMethodService _lciaMethodService;

        private T verifiedDependency<T>(T dependency) where T : class
        {
            if (dependency == null)
                throw new ArgumentNullException("dependency", String.Format("Type: {0}", dependency.GetType().ToString()));
            else
                return dependency;
        }

        /// <summary>
        /// Constructor for XMLHandlerController using dependency injection.
        /// </summary>
        /// <param name="docuService"></param>
        /// <param name="ilcdEntityService"></param>
        /// <param name="processService"></param>
        /// <param name="flowService"></param>
        /// <param name="flowPropertyService"></param>
        /// <param name="lciaMethodService"></param>
        public XMLHandlerController(IDocuService docuService,
            IILCDEntityService ilcdEntityService,
            IProcessService processService,
            IFlowService flowService,
            IFlowPropertyService flowPropertyService,
            ILCIAMethodService lciaMethodService)
        {
            _docuService = verifiedDependency(docuService);
            _ilcdEntityService = verifiedDependency(ilcdEntityService);
            _processService = verifiedDependency(processService);
            _flowService = verifiedDependency(flowService);
            _flowPropertyService = verifiedDependency(flowPropertyService);
            _lciaMethodService = verifiedDependency(lciaMethodService);

        }

        private HttpResponseMessage CreateRedirectResponse(ILCDEntity entity)
        {
            if (entity == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            else
            {
                String uri = (_docuService.UrlRoot(Request) + "xml/" 
                    + Enum.GetName(typeof(DataPathEnum),entity.DataTypeID) // DataTypeEnum and DataPathEnum must coincide
                    + "/" + entity.UUID + "?version=" + entity.Version);

                var response = Request.CreateResponse(HttpStatusCode.Found);
                response.Headers.Location = new Uri(uri, UriKind.Absolute);
                return response;
            }
        }

        private bool TryParse(String uuid, out Guid guid)
        {
            uuid = _docuService.MyTrimEnd(uuid,".xml");
            guid = new Guid();
            if (!Guid.TryParse(uuid, out guid))
                return false;
            return true;
        }

        /// <summary>
        /// Lookup a process by its internal ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>redirect to canonical XML reference</returns>
        [Route("xml/processes/{id:int}")]
        [HttpGet]
        public HttpResponseMessage LookupProcessByID(int id)
        {
            ILCDEntity entity = _processService.Query(k => k.ProcessID == id)
                .Include(k => k.ILCDEntity)
                .Select(k => k.ILCDEntity)
                .FirstOrDefault();

            return CreateRedirectResponse(entity);
        }

        /// <summary>
        /// Lookup a flow by its internal ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>redirect to canonical XML reference</returns>
        [Route("xml/flows/{id:int}")]
        [HttpGet]
        public HttpResponseMessage LookupFlowByID(int id)
        {
            ILCDEntity entity = _flowService.Query(k => k.FlowID == id)
                .Include(k => k.ILCDEntity)
                .Select(k => k.ILCDEntity)
                .FirstOrDefault();

            return CreateRedirectResponse(entity);
        }

        /// <summary>
        /// Lookup a flow property by its internal ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>redirect to canonical XML reference</returns>
        [Route("xml/flowproperties/{id:int}")]
        [HttpGet]
        public HttpResponseMessage LookupFlowPropertyByID(int id)
        {
            ILCDEntity entity = _flowPropertyService.Query(k => k.FlowPropertyID == id)
                .Include(k => k.ILCDEntity)
                .Select(k => k.ILCDEntity)
                .FirstOrDefault();

            return CreateRedirectResponse(entity);
        }

        /// <summary>
        /// Lookup an LCIA method by its internal ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>redirect to canonical XML reference</returns>
        [Route("xml/lciamethods/{id:int}")]
        [HttpGet]
        public HttpResponseMessage LookupLCIAMethodByID(int id)
        {
            ILCDEntity entity = _lciaMethodService.Query(k => k.LCIAMethodID == id)
                .Include(k => k.ILCDEntity)
                .Select(k => k.ILCDEntity)
                .FirstOrDefault();

            return CreateRedirectResponse(entity);
        }

        /// <summary>
        /// Lookup an entity by its internal ID.  Optional version specification as a URL parameter ?version=xx.xx.xxx; 
        /// if no version specified, returns the lexically highest version number.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns>redirect to canonical XML reference</returns>
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

            return CreateRedirectResponse(entity);
        }

        /// <summary>
        /// Canonical XML reference.  Optional version specification as a URL parameter ?version=xx.xx.xxx; 
        /// if no version specified, returns the lexically highest version number.
        /// </summary>
        /// <param name="dpath"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
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
                        String.Format("Incorrect data type. Should be {0} (should be redirect?)",
                        Enum.GetName(typeof(DataPathEnum),(DataPathEnum)entity.DataTypeID)));

                // override default JSON serialization
                Request.Headers.Accept.Remove(new MediaTypeWithQualityHeaderValue("text/html"));
                
                var stringWriter = _ilcdEntityService.GetXmlDocument(entity);

                if (stringWriter == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Source file not found.");

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(stringWriter.ToString(), stringWriter.Encoding, "application/xml");
                response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = new TimeSpan(1,0,0,0) // 1-day cache 
                };
                return response;
            }

        }
    }
}
