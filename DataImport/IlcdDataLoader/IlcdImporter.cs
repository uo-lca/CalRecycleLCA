using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using IlcdDataLoader.Models;

namespace IlcdDataLoader {
    /// <summary>
    /// Use IlcdDataLoader to load ILCD data from a directory to the LCA Tool database.
    /// </summary>
    class IlcdImporter : IDisposable {
        /// <summary>
        /// Wrapper for the app's DbContext
        /// </summary>
        private DbContextWrapper _DbContext = new DbContextWrapper(new LCAToolDbContext());

        /// <summary>
        /// Object to handle loading ILCD data file.
        /// </summary>
        private IlcdData _IlcdData = new IlcdData();

        // Flag: Has Dispose already been called? 
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing) {
            if (disposed)
                return;

            if (disposing) {
                _DbContext.Dispose();
            }

            disposed = true;
        }

        /// <summary>
        /// Load all ILCD data types in directory.
        /// </summary>
        /// <param name="dirName">Full path name of directory</param>
        public void LoadAll(string dirName) {
            LoadDataType(Path.Combine(dirName, "unitgroups"));
        }

        /// <summary>
        /// Load XML files in ILCD data type directory
        /// </summary>
        /// <param name="dirName">Full path name of directory for the data type</param>
        public void LoadDataType(string dirName) {
            if (Directory.Exists(dirName)) {
                string[] files = Directory.GetFiles(dirName, "*.xml");
                foreach (string s in files) {
                    _IlcdData.LoadedDocument = XDocument.Load(s);
                    _IlcdData.Save(_DbContext);
                }
            }
            else {
                Console.WriteLine("ILCD data type folder, {0}, does not exist.", dirName);
            }
        }
    }
}
