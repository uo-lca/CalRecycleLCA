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
        static readonly string _CommonPrefix = _CommonNamespace.ToString();

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

        public XElement GetElementWithInternalId(XName elementName, string internalID) {
            return LoadedDocument.Root
                       .Descendants(elementName)
                       .First(x => x.Attribute("dataSetInternalID").Value == internalID);
        }

        /// <summary>
        /// Generate element name with common namespace prefix
        /// </summary>
        /// <returns>Element name</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public XName CommonElementName(string name) {
            return _CommonNamespace + name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetCommonUUID() {
            return GetElementValue(_CommonNamespace + "UUID");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetCommonName() {
            return GetElementValue(_CommonNamespace + "name");
        }

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

        private List<UnitConversion> CreateUnitConversionList(UnitGroup unitGroup) {

            return LoadedDocument.Root.Descendants(ElementName("units")).Elements(ElementName("unit")).Select(u =>
                    new UnitConversion {
                        UnitConversionUUID = unitGroup.UnitGroupUUID,
                        Unit = (string)u.Element(ElementName("name")),
                        Conversion = (double)u.Element(ElementName("meanValue")),
                        UnitGroupID = unitGroup.UnitGroupID
                    }).ToList();
        }

        private List<FlowFlowProperty> CreateFFPList(DbContextWrapper ilcdDb, Flow flow) {

            return LoadedDocument.Root.Descendants(ElementName("flowProperties")).Elements(ElementName("flowProperty")).Select(fp =>
                    new FlowFlowProperty {
                        FlowID = flow.FlowID,
                        FlowPropertyID = GetFlowPropertyID(ilcdDb, fp),
                        MeanValue = (double?)fp.Element(ElementName("meanValue")),
                        StDev = (double?)fp.Element(ElementName("relativeStandardDeviation95In"))
                    }).ToList();
        }

        private bool SaveUnitGroup(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            UnitGroup unitGroup = new UnitGroup();
            unitGroup.UnitGroupUUID = GetCommonUUID();
            unitGroup.Version = GetCommonVersion();
            unitGroup.Name = GetCommonName();
            if (ilcdDb.AddUnitGroup(unitGroup)) {

                    ilcdDb.AddUnitConversions(CreateUnitConversionList(unitGroup));
                    isSaved = true;
            }
            return isSaved;
        }

        private bool SaveFlowProperty(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            string ugUUID;
            int? ugID = null;
            FlowProperty flowProperty = new FlowProperty();
            flowProperty.FlowPropertyUUID = GetCommonUUID();
            flowProperty.FlowPropertyVersion = GetCommonVersion();
            flowProperty.Name = GetCommonName();
            ugUUID = GetElementAttributeValue(ElementName("referenceToReferenceUnitGroup"), "refObjectId");
            if (ugUUID == null) {
                Console.WriteLine("WARNING: Unable to find referenceToReferenceUnitGroup in flow property {0}", 
                    flowProperty.FlowPropertyUUID);
            }
            else {
                string referenceUUID = ugUUID;
                ugID = ilcdDb.GetID((string)referenceUUID);
                if (ugID == null) {
                    Console.WriteLine("WARNING: Unable to find unit group matching flow property refObjectId = {0}", ugUUID);
                }
                else {
                    flowProperty.UnitGroupID = ugID;
                }
            }

            if (ilcdDb.AddFlowProperty(flowProperty) ) {
                isSaved = true;
            }
            
            return isSaved;
        }

        private int? GetFlowPropertyID(DbContextWrapper ilcdDb, XElement fpElement) {
            string fpUUID = fpElement.Element(ElementName("referenceToFlowPropertyDataSet")).Attribute("refObjectId").Value;
            int? fpID = ilcdDb.GetID(fpUUID);
            if (fpID == null) {
                Console.WriteLine("WARNING: Unable to find flow property matching flow refObjectId = {0}", fpUUID);
            }
            return fpID;
        }

        private bool SaveFlow(DbContextWrapper ilcdDb) {
            bool isSaved = false;
            int? fpID;
            string dataSetInternalID = "0";
            XElement fpElement;
            Flow flow = new Flow();
            flow.FlowUUID = GetCommonUUID();
            flow.FlowVersion = GetCommonVersion();
            // TODO : generate name from classification/category
            flow.Name = GetElementValue(ElementName("baseName"));
            flow.CASNumber = GetElementValue(ElementName("CASNumber"));
            flow.FlowTypeID = ilcdDb.GetFlowTypeID(GetElementValue(ElementName("typeOfDataSet")));
            // Get Reference Flow Property
            dataSetInternalID = GetElementValue(ElementName("referenceToReferenceFlowProperty"));
            fpElement = GetElementWithInternalId(ElementName("flowProperty"), dataSetInternalID);
            fpID = GetFlowPropertyID(ilcdDb, fpElement);
            flow.FlowPropertyID = fpID;           

            if (ilcdDb.AddFlow(flow)) {
                ilcdDb.AddFlowFlowProperties(CreateFFPList(ilcdDb, flow));
                isSaved = true;
            }

            return isSaved;
        }

        /// <summary>
        /// Import data from LoadedDocument to database.
        /// </summary>
        public bool Save(DbContextWrapper ilcdDb) {
            Debug.Assert(LoadedDocument != null, "LoadedDocument must be set before calling Save.");
            string nsString = LoadedDocument.Root.Name.Namespace.ToString();
            switch (nsString) {
                case "http://lca.jrc.it/ILCD/UnitGroup":
                    return SaveUnitGroup(ilcdDb);
                case "http://lca.jrc.it/ILCD/FlowProperty":
                    return SaveFlowProperty(ilcdDb);
                case "http://lca.jrc.it/ILCD/Flow":
                    return SaveFlow(ilcdDb);
            }
            return false;
        }

    }
}
