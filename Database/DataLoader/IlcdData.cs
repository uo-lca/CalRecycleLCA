using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using LcaDataModel;

namespace LcaDataLoader {
    /// <summary>
    /// Use IlcdData to import new ILCD data in an XDocument to a record in the database. 
    /// </summary>
    class IlcdData {
        static readonly XNamespace _CommonNamespace = "http://lca.jrc.it/ILCD/Common";
        static readonly XNamespace _GabiNamespace = "http://www.pe-international.com/GaBi";

        /// <summary>
        /// XDocument returned from Load of ILCD XML file
        /// </summary>
        public XDocument LoadedDocument { get; set; }

        // Cache for use in references - map UUID to specific entity ID 
        Dictionary<string, int> _EntityIdDictionary = new Dictionary<string, int>();

        /// <summary>
        /// Find descendant element with given name and return its value
        /// </summary>
        /// <returns>Element value as a string</returns>
        public string GetElementValue(XName elementName) {
            IEnumerable<XElement> els =
                from el in LoadedDocument.Root.Descendants(elementName)
                select el;
            if (els.Count() == 0) {
                return null;
            }
            else {
                return LoadedDocument.Root
                           .Descendants(elementName)
                           .Select(s => s.Value)
                           .First();
            }
        }

        /// <summary>
        /// Find descendant element with given name and return value of attribute with given name
        /// </summary>
        /// <returns>Attribute value as a string</returns>
        public string GetElementAttributeValue(XName elementName, XName attName) {
            IEnumerable<XElement> els =
                from el in LoadedDocument.Root.Descendants(elementName)
                select el;
            if (els.Count() == 0) {
                return null;
            }
            else {
                return els.Attributes(attName)
                       .Select(s => s.Value)
                       .First();
            }             
        }

        /// <summary>
        /// Find descendant element by dataSetInternalID value
        /// </summary>
        /// <param name="elementName">Element name of parent node</param>
        /// <param name="internalID">Search value</param>
        /// <returns>Element found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public XElement GetElementWithInternalId(XName elementName, string internalID) {
            return LoadedDocument.Root
                       .Descendants(elementName)
                       .First(x => x.Attribute("dataSetInternalID").Value == internalID);
        }

        /// <summary>
        /// Get UUID using common namespace prefix
        /// </summary>
        /// <returns>Value of the UUID</returns>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetCommonUUID() {
            return GetElementValue(_CommonNamespace + "UUID");
        }

        /// <summary>
        /// Get name using common namespace prefix
        /// </summary>
        /// <returns>Value of the name</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetCommonName() {
            return GetElementValue(_CommonNamespace + "name");
        }

        /// <summary>
        /// Get dataSetVersion using common namespace prefix
        /// </summary>
        /// <returns>Value of the dataSetVersion</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetCommonVersion() {
            return GetElementValue(_CommonNamespace + "dataSetVersion");
        }

        /// <summary>
        /// Get element name with current namespace prefix
        /// </summary>
        /// <returns>Element name</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public XName ElementName(string name) {
            return LoadedDocument.Root.Name.Namespace + name;
        }

        /// <summary>
        /// Create a list of UnitConversion entities from elements under units.
        /// </summary>
        /// <param name="flow">UnitGroup parent entity</param>
        /// <param name="refID">unit group referenceToReferenceUnit</param>
        private List<UnitConversion> CreateUnitConversionList(UnitGroup unitGroup, string refID) {
            List<UnitConversion> ucList = new List<UnitConversion>();
            IEnumerable<XElement> elements = LoadedDocument.Root.Descendants(ElementName("units")).Elements(ElementName("unit"));
            foreach (XElement el in elements) {
                UnitConversion uc = new UnitConversion {
                        Unit = (string)el.Element(ElementName("name")),
                        Conversion = (double)el.Element(ElementName("meanValue")),
                        UnitGroupID = unitGroup.UnitGroupID
                    };
                if ( el.Attribute("dataSetInternalID").Value == refID) {
                    unitGroup.UnitConversion = uc;
                }
                ucList.Add(uc);
            }
            return ucList;
        }

        /// <summary>
        /// Create a list of FlowFlowProperty entities from elements under flowProperties.
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <param name="flow">Flow parent entity</param>
        private List<FlowFlowProperty> CreateFFPList(DbContextWrapper ilcdDb, Flow flow) {
            return LoadedDocument.Root.Descendants(ElementName("flowProperties")).Elements(ElementName("flowProperty")).Select(fp =>
                    new FlowFlowProperty {
                        FlowID = flow.FlowID,
                        FlowPropertyID = GetFlowPropertyID(ilcdDb, fp),
                        MeanValue = (double?)fp.Element(ElementName("meanValue")),
                        StDev = (double?)fp.Element(ElementName("relativeStandardDeviation95In"))
                    }).ToList();
        }

        /// <summary>
        /// Create an LCIA entity from an LCIAMethod characterization factor.
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <param name="el">LCIAMethod characterization factor element</param>
        /// <param name="lciaMethodID">LCIAMethod parent entity ID</param>
        private LCIA CreateLCIA(DbContextWrapper ilcdDb, XElement el, int lciaMethodID) {
            LCIA lcia;
            XElement refEl = el.Element(ElementName("referenceToFlowDataSet"));
            string uuid = refEl.Attribute("refObjectId").Value;
            double meanValue = (double)el.Element(ElementName(("meanValue")));
            string direction = (string)el.Element(ElementName(("exchangeDirection")));
            string location = (string)el.Element(ElementName(("location")));
            string name = (string)refEl.Element(_CommonNamespace + "shortDescription");
            Flow flow = ilcdDb.GetIlcdEntity<Flow>(uuid);
            int? id = null;
            if (flow == null) {
                Program.Logger.WarnFormat("Unable to find flow matching LCIA refObjectId = {0}", uuid);
            }
            else {
                id = flow.FlowID;
            }
            int? dirID = ilcdDb.LookupEntityID<Direction>(direction);
            if (dirID == null) {
                Program.Logger.ErrorFormat("Unable to find ID for LCIA exchangeDirection = {0}", direction);
            }
            lcia = new LCIA { FlowID = id, FlowUUID = uuid, FlowName = name,
                              DirectionID = dirID, Factor = meanValue, Geography = location, LCIAMethodID = lciaMethodID };
            return lcia;
        }

        /// <summary>
        /// Create a process flow entity from an Process exchange.
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <param name="el">Process exchange element</param>
        /// <param name="processID">Process parent entity ID</param>
        private ProcessFlow CreateProcessFlow(DbContextWrapper ilcdDb, XElement el, int processID) {
            string type = (string)el.Element(ElementName("referenceToVariable"));
            string varName = (string)el.Element(_CommonNamespace + "other").Element(_GabiNamespace + "GaBi").Attribute("IOType");
            double magnitude = (double)el.Element(ElementName("meanAmount"));
            double result = (double)el.Element(ElementName("resultingAmount"));
            double stdev = (double)el.Element(ElementName("relativeStandardDeviation95In"));
            string uuid = el.Element(ElementName("referenceToFlowDataSet")).Attribute("refObjectId").Value;
            int? id = ilcdDb.GetIlcdEntityID<Flow>(uuid);
            if (id == null) {
                Program.Logger.WarnFormat("Unable to find flow matching exchange refObjectId = {0}", uuid);
            }
            string direction = (string)el.Element(ElementName(("exchangeDirection")));
            int? dirID = ilcdDb.LookupEntityID<Direction>(direction);
            if (dirID == null) {
                Program.Logger.WarnFormat("Unable to find ID for exchangeDirection = {0}", direction);
            }
            string location = (string)el.Element(ElementName(("location")));
            return new ProcessFlow { 
                DirectionID = dirID, FlowID = id, Geography = location, 
                Magnitude = magnitude, ProcessID = processID, Result = result,
                STDev = stdev, Type = type, VarName = varName };
        }
       
        /// <summary>
        /// Import common ILCD data from loaded ILCD file to new ILCDEntity object.
        /// Save UUID and reference to new object in object implementing IIlcdEntity
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <param name="entity">Object implementing IIlcdEntity</param>
        /// <param name="dtEnum">Data type of entity</param>
        /// <returns>new ILCDEntity object<returns>
        private ILCDEntity SaveIlcdEntity(DbContextWrapper ilcdDb, IIlcdEntity entity, DataTypeEnum dtEnum) {
            ILCDEntity ilcdEntity = new ILCDEntity();
            ilcdEntity.UUID = GetCommonUUID();
            ilcdEntity.Version = GetCommonVersion();
            ilcdEntity.DataTypeID = Convert.ToInt32(dtEnum);
            ilcdEntity.DataProviderID = ilcdDb.GetCurrentIlcdDataProviderID();
            //entity.UUID = GetCommonUUID();
            entity.ILCDEntity = ilcdEntity;
            return ilcdEntity;
        }

        /// <summary>
        /// Import data from loaded unitgroup file to new UnitGroup entity
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <returns>true iff data was imported</returns>
        private bool SaveUnitGroup(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            string dataSetInternalID = "0";
            string uuid = GetCommonUUID();
            int? entityID = ilcdDb.GetIlcdEntityID<UnitGroup>(uuid);
            if (entityID == null) {
                UnitGroup unitGroup = new UnitGroup();
                SaveIlcdEntity(ilcdDb, unitGroup, DataTypeEnum.UnitGroup);
                unitGroup.Name = GetCommonName();
                // Get Reference Flow Property
                dataSetInternalID = GetElementValue(ElementName("referenceToReferenceUnit"));
                if (ilcdDb.AddIlcdEntity(unitGroup)) {
                    _EntityIdDictionary[uuid] = unitGroup.UnitGroupID;
                    ilcdDb.AddEntities<UnitConversion>(CreateUnitConversionList(unitGroup, dataSetInternalID));
                    isSaved = true;
                }
            }
            else {
                Program.Logger.WarnFormat("Unit Group with UUID {0} was already imported and will not be updated.", uuid);
                _EntityIdDictionary[uuid] = Convert.ToInt32(entityID);
            }
            
            return isSaved;
        }

        /// <summary>
        /// Search for entity by UUID and output its specific entity ID. 
        /// Search local cache first, then database. 
        /// If found in database, add to cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ilcdDb">Current DbContextWrapper instance</param>
        /// <param name="uuid">The UUID</param>
        /// <param name="outID">Output: Entity ID in its own table</param>
        /// <returns>true iff the entity was found</returns>
        private bool TryGetIlcdEntityID<T>(DbContextWrapper ilcdDb, string uuid, out int outID) where T : class, IIlcdEntity {
            bool found = _EntityIdDictionary.TryGetValue(uuid, out outID);
            if ( !found ) {
                int? nid = ilcdDb.GetIlcdEntityID<T>(uuid);
                if (nid != null) {
                    outID = Convert.ToInt32(nid);
                    _EntityIdDictionary[uuid] = outID;
                    found = true;
                }
            }
            return found;
        }

        /// <summary>
        /// Import data from loaded flowproperty file to new FlowProperty entity
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <returns>true iff data was imported</returns>
        private bool SaveFlowProperty(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            string ugUUID;            
            string uuid = GetCommonUUID();
            int? entityID = ilcdDb.GetIlcdEntityID<FlowProperty>(uuid);
            if (entityID == null) {
                FlowProperty flowProperty = new FlowProperty();
                SaveIlcdEntity(ilcdDb, flowProperty, DataTypeEnum.FlowProperty);
                flowProperty.Name = GetCommonName();
                ugUUID = GetElementAttributeValue(ElementName("referenceToReferenceUnitGroup"), "refObjectId");
                if (ugUUID == null) {
                    Program.Logger.WarnFormat("Unable to find referenceToReferenceUnitGroup in flow property {0}",
                        flowProperty.ILCDEntity.UUID);
                }
                else {
                    int ugID;
                    if (TryGetIlcdEntityID<UnitGroup>(ilcdDb, ugUUID, out ugID)) {
                        flowProperty.UnitGroupID = ugID;
                    } else {
                        Program.Logger.ErrorFormat("Unable to find unit group matching flow property refObjectId = {0}", ugUUID);
                    } 
                }

                if (ilcdDb.AddIlcdEntity(flowProperty)) {
                    _EntityIdDictionary[uuid] = flowProperty.FlowPropertyID;
                    isSaved = true;
                }
            }
            else {
                Program.Logger.WarnFormat("FlowProperty with UUID {0} was already imported and will not be updated.", uuid);
                _EntityIdDictionary[uuid] = Convert.ToInt32(entityID);
            }
            
            return isSaved;
        }

        /// <summary>
        /// Extract UUID from referenceToFlowPropertyDataSet and transform it to entity ID (FlowPropertyID).
        /// This depends on the referenced flow property having been previously imported.
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <param name="fpElement">Element containing referenceToFlowPropertyDataSet</param>
        /// <returns>Entity ID, if the UUID was extracted and a loaded entity ID was found, otherwise null</returns>
        private int? GetFlowPropertyID(DbContextWrapper ilcdDb, XElement fpElement) {
            string fpUUID = fpElement.Element(ElementName("referenceToFlowPropertyDataSet")).Attribute("refObjectId").Value;
            int fpID;
            if (TryGetIlcdEntityID<FlowProperty>(ilcdDb, fpUUID, out fpID)) {
                return fpID;
            }
            else {
                Program.Logger.ErrorFormat("Unable to find flow property matching flow refObjectId = {0}", fpUUID);
                return null;
            }
        }


        /// <summary>
        /// Import data from loaded flow file to new Flow entity
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <returns>true iff data was imported</returns>
        private bool SaveFlow(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            int? fpID;
            string dataSetInternalID = "0";
            string uuid = GetCommonUUID();
            int? entityID = ilcdDb.GetIlcdEntityID<FlowProperty>(uuid);
            if (entityID == null) {
                XElement fpElement;
                Flow flow = new Flow();
                SaveIlcdEntity(ilcdDb, flow, DataTypeEnum.Flow);
                // TODO : generate name from classification/category
                flow.Name = GetElementValue(ElementName("baseName"));
                flow.CASNumber = GetElementValue(ElementName("CASNumber"));
                flow.FlowTypeID = ilcdDb.GetFlowTypeID(GetElementValue(ElementName("typeOfDataSet")));
                // Get Reference Flow Property
                dataSetInternalID = GetElementValue(ElementName("referenceToReferenceFlowProperty"));
                fpElement = GetElementWithInternalId(ElementName("flowProperty"), dataSetInternalID);
                fpID = GetFlowPropertyID(ilcdDb, fpElement);
                flow.ReferenceFlowProperty = fpID;

                if (ilcdDb.AddIlcdEntity(flow)) {
                    _EntityIdDictionary[uuid] = flow.FlowID;
                    ilcdDb.AddEntities<FlowFlowProperty>(CreateFFPList(ilcdDb, flow));
                    isSaved = true;
                }
            }
            else {
                Program.Logger.WarnFormat("Flow with UUID {0} was already imported and will not be updated.", uuid);
                _EntityIdDictionary[uuid] = Convert.ToInt32(entityID);
            }

            return isSaved;
        }

        private bool SaveLciaMethod(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            string lookupName;
            string refUUID;
            string uuid = GetCommonUUID();
            int? entityID = ilcdDb.GetIlcdEntityID<FlowProperty>(uuid);
            if (entityID == null) {
                LCIAMethod lciaMethod = new LCIAMethod();
                SaveIlcdEntity(ilcdDb, lciaMethod, DataTypeEnum.LCIAMethod);
                lciaMethod.Name = GetCommonName();
                lciaMethod.Methodology = GetElementValue(ElementName("methodology"));
                lookupName = GetElementValue(ElementName("impactCategory"));
                if (lookupName != null) {
                    lciaMethod.ImpactCategoryID = ilcdDb.LookupEntityID<ImpactCategory>(lookupName);
                }
                lciaMethod.ImpactIndicator = GetElementValue(ElementName("impactIndicator"));
                lookupName = GetElementValue(ElementName("typeOfDataSet"));
                if (lookupName != null) {
                    lciaMethod.IndicatorTypeID = ilcdDb.LookupEntityID<IndicatorType>(lookupName);
                }
                lciaMethod.ReferenceYear = GetElementValue(ElementName("referenceYear"));
                lciaMethod.Duration = GetElementValue(ElementName("duration"));
                lciaMethod.ImpactLocation = GetElementValue(ElementName("impactLocation"));
                lciaMethod.Normalization = Convert.ToBoolean(GetElementValue(ElementName("normalisation")));
                lciaMethod.Weighting = Convert.ToBoolean(GetElementValue(ElementName("weighting")));
                lciaMethod.UseAdvice = GetElementValue(ElementName("useAdviceForDataSet"));
                refUUID = GetElementAttributeValue(ElementName("referenceQuantity"), "refObjectId");
                Debug.Assert(refUUID != null);
                lciaMethod.ReferenceQuantity = ilcdDb.GetIlcdEntityID<FlowProperty>(refUUID);
                if (ilcdDb.AddIlcdEntity(lciaMethod)) {
                    List<LCIA> lciaList =
                        LoadedDocument.Root.Descendants(ElementName("characterisationFactors")).Elements(ElementName("factor")).Select(f =>
                            CreateLCIA(ilcdDb, f, lciaMethod.ID)).ToList();
                    ilcdDb.AddEntities<LCIA>(lciaList);

                    isSaved = true;

                }
                else {
                    Program.Logger.WarnFormat("LCIA Method with UUID {0} was already imported and will not be updated.", uuid);
                    _EntityIdDictionary[uuid] = Convert.ToInt32(entityID);
                }
            }
            return isSaved;
        }

        private bool SaveProcess(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            string lookupName;
            LcaDataModel.Process process = new LcaDataModel.Process();
            SaveIlcdEntity(ilcdDb, process, DataTypeEnum.Process);
            process.Name = GetElementValue(ElementName("baseName"));
            process.ReferenceYear = GetElementValue(_CommonNamespace + "referenceYear");
            process.Geography = GetElementAttributeValue(ElementName("locationOfOperationSupplyOrProduction"), "location");            
            lookupName = GetElementAttributeValue(ElementName("quantitativeReference"), "type");
            if (lookupName != null) {
                process.ReferenceTypeID = ilcdDb.LookupEntityID<ReferenceType>(lookupName);
            }
            lookupName = GetElementValue(ElementName("typeOfDataSet"));
            if (lookupName != null) {
                process.ProcessTypeID = ilcdDb.LookupEntityID<ProcessType>(lookupName);
            }
            if (ilcdDb.AddIlcdEntity(process)) {
                List<ProcessFlow> pfList =
                    LoadedDocument.Root.Descendants(ElementName("exchanges")).Elements(ElementName("exchange")).Select(f =>
                        CreateProcessFlow(ilcdDb, f, process.ID)).ToList();
                ilcdDb.AddEntities<ProcessFlow>(pfList);

                isSaved = true;
            }
            return isSaved;
        }

        /// <summary>
        /// Import data from LoadedDocument to database.
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <returns>true iff data was imported</returns>
        public bool Save(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            Debug.Assert(LoadedDocument != null, "LoadedDocument must be set before calling Save.");
            string nsString = LoadedDocument.Root.Name.Namespace.ToString();
            string commonUUID = GetCommonUUID();
            if (ilcdDb.IlcdUuidExists(commonUUID)) {
                Program.Logger.WarnFormat("UUID {0} was already imported and will not be updated.", commonUUID);
            }
            else {
                switch (nsString) {
                    case "http://lca.jrc.it/ILCD/UnitGroup":
                        isSaved = SaveUnitGroup(ilcdDb);
                        break;
                    case "http://lca.jrc.it/ILCD/FlowProperty":
                        isSaved = SaveFlowProperty(ilcdDb);
                        break;
                    case "http://lca.jrc.it/ILCD/Flow":
                        isSaved = SaveFlow(ilcdDb);
                        break;
                    case "http://lca.jrc.it/ILCD/LCIAMethod":
                        isSaved = SaveLciaMethod(ilcdDb);
                        break;
                    case "http://lca.jrc.it/ILCD/Process":
                        isSaved = SaveProcess(ilcdDb);
                        break;
                }
            }
            return isSaved;
        }

    }
}
