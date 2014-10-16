using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;

namespace LcaDataLoader {
    /// <summary>
    /// Use IlcdImporter to load ILCD data from a directory to the LCA Tool database.
    /// </summary>
    class IlcdImporter  {
        /// <summary>
        /// Object to handle loading ILCD data file.
        /// </summary>
        private IlcdData _IlcdData = new IlcdData();

        /// <summary>
        /// Load all ILCD data types in directory.
        /// </summary>
        /// <param name="dirName">Full path name of directory</param>
        /// <param name="ilcdSourceName">Name of ILCD data source</param>
        /// <param name="dbContext">Shared instance of DbContextWrapper</param>
        public void LoadAll(string dirName, string ilcdSourceName, DbContextWrapper dbContext) {
            if (dbContext.CreateDataSource(dirName, ilcdSourceName) != null) {
                // Improve load performance by disabling AutoDetectChanges.
                dbContext.SetAutoDetectChanges(false);
                LoadDataType(Path.Combine(dirName, "unitgroups"), dbContext);
                LoadDataType(Path.Combine(dirName, "flowproperties"), dbContext);
                LoadDataType(Path.Combine(dirName, "flows"), dbContext);
                LoadDataType(Path.Combine(dirName, "LCIAmethods"), dbContext);
                LoadDataType(Path.Combine(dirName, "processes"), dbContext);
                dbContext.SetAutoDetectChanges(true);
            }
        }

        /// <summary>
        /// Load XML files in ILCD data type directory
        /// </summary>
        /// <param name="dirName">Full path name of directory for the data type</param>
        /// <param name="dbContext">Instance of DbContextWrapper</param>
        public void LoadDataType(string dirName, DbContextWrapper dbContext) {
            int importCounter = 0;
            if (Directory.Exists(dirName)) {
                Program.Logger.InfoFormat("Load {0}...", dirName);
                string[] files = Directory.GetFiles(dirName, "*.xml");
                foreach (string s in files) {
                    Program.Logger.DebugFormat("Load {0}", s);
                    _IlcdData.LoadedDocument = XDocument.Load(s);
                    if (_IlcdData.Save(dbContext)) {
                        importCounter++;
                    }
                    else {
                        Program.Logger.WarnFormat("Data in file {0} was not imported.", s);
                    }
                }
                Program.Logger.InfoFormat("{0} of {1} files imported from {2}.", importCounter, files.Length, dirName);
            }
            else {
                Program.Logger.WarnFormat("ILCD data type folder, {0}, does not exist.", dirName);
            }
        }
    }
}
