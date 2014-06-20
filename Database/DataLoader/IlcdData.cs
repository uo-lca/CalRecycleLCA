﻿using System;
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
        private List<UnitConversion> CreateUnitConversionList(UnitGroup unitGroup) {
            return LoadedDocument.Root.Descendants(ElementName("units")).Elements(ElementName("unit")).Select(u =>
                    new UnitConversion {
                        Unit = (string)u.Element(ElementName("name")),
                        Conversion = (double)u.Element(ElementName("meanValue")),
                        UnitGroupID = unitGroup.UnitGroupID
                    }).ToList();
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
            string uuid = el.Element(ElementName("referenceToFlowDataSet")).Attribute("refObjectId").Value;
            double meanValue = (double)el.Element(ElementName(("meanValue")));
            string direction = (string)el.Element(ElementName(("exchangeDirection")));
            string location = (string)el.Element(ElementName(("location")));
            int? id = ilcdDb.GetIlcdEntityID<Flow>(uuid);
            if (id == null) {
                Program.Logger.WarnFormat("Unable to find flow matching LCIA refObjectId = {0}", uuid);
            }
            int? dirID = ilcdDb.LookupEntityID<Direction>(direction);
            if (dirID == null) {
                Program.Logger.WarnFormat("Unable to find ID for exchangeDirection = {0}", direction);
            }
            lcia = new LCIA { FlowID = id, DirectionID = dirID, Factor = meanValue, Geography = location, LCIAMethodID = lciaMethodID };
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
        /// <returns>new ILCDEntity object<returns>
        private ILCDEntity SaveIlcdEntity(DbContextWrapper ilcdDb, IIlcdEntity entity) {
            ILCDEntity ilcdEntity = new ILCDEntity();
            ilcdEntity.UUID = GetCommonUUID();
            ilcdEntity.Version = GetCommonVersion();
            entity.UUID = GetCommonUUID();
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
            UnitGroup unitGroup = new UnitGroup();
            SaveIlcdEntity(ilcdDb, unitGroup);
            unitGroup.Name = GetCommonName();
            if (ilcdDb.AddIlcdEntity(unitGroup)) {
                    ilcdDb.AddEntities<UnitConversion>(CreateUnitConversionList(unitGroup));
                    isSaved = true;
            }
            return isSaved;
        }

        /// <summary>
        /// Import data from loaded flowproperty file to new FlowProperty entity
        /// </summary>
        /// <param name="ilcdDb">Database context wrapper object</param>
        /// <returns>true iff data was imported</returns>
        private bool SaveFlowProperty(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            string ugUUID;
            int? ugID = null;
            FlowProperty flowProperty = new FlowProperty();
            SaveIlcdEntity(ilcdDb, flowProperty);
            flowProperty.Name = GetCommonName();
            ugUUID = GetElementAttributeValue(ElementName("referenceToReferenceUnitGroup"), "refObjectId");
            if (ugUUID == null) {
                Program.Logger.WarnFormat("Unable to find referenceToReferenceUnitGroup in flow property {0}", 
                    flowProperty.UUID);
            }
            else {
                string referenceUUID = ugUUID;
                ugID = ilcdDb.GetIlcdEntityID<UnitGroup>((string)referenceUUID);
                if (ugID == null) {
                    Program.Logger.WarnFormat("Unable to find unit group matching flow property refObjectId = {0}", ugUUID);
                }
                else {
                    flowProperty.UnitGroupID = ugID;
                }
            }

            if (ilcdDb.AddIlcdEntity(flowProperty)) {
                isSaved = true;
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
            int? fpID = ilcdDb.GetIlcdEntityID<FlowProperty>(fpUUID);
            if (fpID == null) {
                Program.Logger.WarnFormat("Unable to find flow property matching flow refObjectId = {0}", fpUUID);
            }
            return fpID;
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
            XElement fpElement;
            Flow flow = new Flow();
            SaveIlcdEntity(ilcdDb, flow);
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
                ilcdDb.AddEntities<FlowFlowProperty>(CreateFFPList(ilcdDb, flow));
                isSaved = true;
            }

            return isSaved;
        }

        private bool SaveLciaMethod(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            string lookupName;
            string refUUID;
            LCIAMethod lciaMethod = new LCIAMethod();
            SaveIlcdEntity(ilcdDb, lciaMethod);
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
            return isSaved;
        }

        private bool SaveProcess(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            string lookupName;
            LcaDataModel.Process process = new LcaDataModel.Process();
            SaveIlcdEntity(ilcdDb, process);
            process.Name = GetElementValue(ElementName("baseName"));
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