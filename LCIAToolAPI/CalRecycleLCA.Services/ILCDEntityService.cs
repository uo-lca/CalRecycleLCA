using System;
using System.Configuration;
using System.Xml.Linq;
using System.IO;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;

namespace CalRecycleLCA.Services
{
    public interface IILCDEntityService : IService<ILCDEntity>
    {
        List<ILCDEntity> LookupUUID(string uuid);
        ILCDEntity LookupUUID(string uuid, string version);
        StringWriter GetXmlDocument(ILCDEntity entity);
    }

    public class ILCDEntityService : Service<ILCDEntity>, IILCDEntityService
    {
        // need to figure out how to configurate this
        private String DataRoot = ConfigurationManager.AppSettings["DataRoot"];
        
        private IRepository<ILCDEntity> _repository;
        public ILCDEntityService(IRepositoryAsync<ILCDEntity> repository)
            : base(repository)
        {
            _repository = repository; 
        }

        public List<ILCDEntity> LookupUUID(String uuid)
        {
            // assume it's already a valid UUID
            return _repository.Query(k => k.UUID == uuid)
                .Include(k => k.DataSource)
                .Select()
                .OrderBy(k => k.Version)
                .ToList();
        }
    
        public ILCDEntity LookupUUID(String uuid, String version)
        {
            // assume it's already a valid UUID
            return _repository.Query(k => k.UUID == uuid && String.Compare(k.Version, version, StringComparison.OrdinalIgnoreCase) == 0)
                .Include(k => k.DataSource)
                .Select()
                .First();
        }

        public StringWriter GetXmlDocument(ILCDEntity entity)
        {
            var sw = new StringWriter();
            // note: entity must be Eager-queried to .Include DataSource
            var filePath = DataRoot + entity.DataSource.Name + "/ILCD/"
                + Enum.GetName(typeof(DataPathEnum), (DataPathEnum)entity.DataTypeID) + "/"
                + entity.UUID + ".xml";

            if (!File.Exists(filePath))
                return null;

            XDocument xmlDocument = XDocument.Load(filePath);

            if (entity.DataSource.VisibilityID == Convert.ToInt32(VisibilityEnum.Private))
            {
                var ns = xmlDocument.Root.Name.Namespace;
                // remove private elements: all exchanges except the reference flow
                XElement refFlow = xmlDocument.Root.Descendants(ns + "referenceToReferenceFlow").FirstOrDefault();
                if (refFlow == null)
                    xmlDocument.Descendants(ns + "exchanges").Remove();
                else
                    xmlDocument.Descendants(ns + "exchange").Where(k => k.Attribute("dataSetInternalID").Value != refFlow.Value).Remove();
            }

            xmlDocument.Save(sw);

            return sw;

        }
    }
}
