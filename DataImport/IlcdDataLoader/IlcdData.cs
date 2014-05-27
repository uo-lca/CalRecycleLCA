using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using IlcdDataLoader.Models;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;

namespace IlcdDataLoader {
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
            string elementValue;
            elementValue = LoadedDocument.Root
                       .Descendants(elementName)
                       .Select(s => s.Value)
                       .First();
            return elementValue;
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

        private void SaveUnitGroup(DbContextWrapper ilcdDb) {
            UnitGroup unitGroup = new UnitGroup();
            unitGroup.UnitGroupUUID = GetCommonUUID();
            unitGroup.Version = GetCommonVersion();
            unitGroup.Name = GetCommonName();
            ilcdDb.AddUnitGroup(unitGroup);
            if ( ilcdDb.SaveChanges() > 0) {
                ilcdDb.AddUnitConversions( CreateUnitConversionList(unitGroup));
                ilcdDb.SaveChanges();
            }
        }

        /// <summary>
        /// Import data from LoadedDocument to database.
        /// </summary>
        public void Save(DbContextWrapper ilcdDb) {
            Debug.Assert(LoadedDocument != null, "LoadedDocument must be set before calling Save.");
            string nsString = LoadedDocument.Root.Name.Namespace.ToString();
            switch (nsString) {
                case "http://lca.jrc.it/ILCD/UnitGroup":
                    SaveUnitGroup(ilcdDb);
                    break;

            }
        }

    }
}
