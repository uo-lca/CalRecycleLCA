
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccess;
using LcaDataModel;

namespace LcaDataLoader {
    class CsvImporter {

        /// <summary>
        /// Load all csv directories under a given root directory.
        /// </summary>
        /// <param name="dirName">Full path name of the root directory</param>
        public void LoadAll(string dirName) {
            using (DbContextWrapper dbContext = new DbContextWrapper()) {
                LoadAppend(Path.Combine(dirName, "append"), dbContext);
                LoadFragments(Path.Combine(dirName, "fragments"), dbContext);
            }
        }

        private bool ImportCategorySystem(Row row, DbContextWrapper dbContext) {
            CategorySystem obj = new CategorySystem {
                DataTypeID = Convert.ToInt32(row["DataTypeID"]),
                Delimiter = row["Delimiter"],
                Name = row["Name"]
            } ;   
            return dbContext.AddEntity(obj);
        }

        private bool ImportCategory(Row row, DbContextWrapper dbContext) {
            int parentID = Convert.ToInt32(row["ParentCategoryID"]);
            Category obj = new Category {
                CategorySystemID = Convert.ToInt32(row["CategorySystemID"]),
                ExternalClassID = row["ExternalClassID"],
                HierarchyLevel = Convert.ToInt32(row["HierarchyLevel"]),
                Name = row["Name"],
            };
            if (parentID > 0) {
                obj.ParentCategoryID = parentID;
            }
            return dbContext.AddEntity(obj);
        }

        private bool ImportClassification(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            string uuid = row["UUID"];
            if (dbContext.IlcdUuidExists(uuid)) {
                Classification obj = new Classification {
                    UUID = uuid,
                    CategoryID = Convert.ToInt32(row["CategoryID"])
                };
                isImported = dbContext.AddEntity(obj);
            }
            else {
                Program.Logger.WarnFormat("Classification UUID {0} not found. Skipping record.", uuid);
            }
            return isImported;
        }

        private bool CreateIlcdEntity(string uuid, DataProviderEnum providerEnum, DataTypeEnum typeEnum, DbContextWrapper dbContext) {
            ILCDEntity ilcdEntity = new ILCDEntity {
                UUID = uuid,
                DataProviderID = Convert.ToInt32(providerEnum),
                DataTypeID = Convert.ToInt32(typeEnum)
            };
            return dbContext.AddEntity(ilcdEntity);
        }

        private bool CreateFragment(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            string uuid = row["FragmentUUID"];
            if (dbContext.IlcdUuidExists(uuid)) {
                Program.Logger.WarnFormat("UUID {0} already exists. Fragment will not be created.", uuid);
            }
            else if ( CreateIlcdEntity( uuid, DataProviderEnum.fragments, DataTypeEnum.Fragment, dbContext)) {
                Fragment obj = new Fragment {
                    UUID = uuid,
                    Name = row["Name"]
                };
                isImported = dbContext.AddEntity(obj);
            }        
            return isImported;
        }

        private int ImportCSV(string fileName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext) {
            int importCounter = 0;
            var table = DataAccess.DataTable.New.ReadCsv(fileName);
            foreach (Row row in table.Rows) {
                if (importRow(row, dbContext)) importCounter++;
            }
            Program.Logger.InfoFormat("{0} of {1} records imported from {2}.", importCounter, table.Rows.Count(), fileName);
            return importCounter;
        }

        private bool ImportAppendCSV(string dirName, string typeName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext) {
            string fileName = Path.Combine(dirName, typeName + ".csv");
            if (System.IO.File.Exists(fileName)) {
                if (ImportCSV(fileName, importRow, dbContext) > 0) {
                    System.IO.File.Move(fileName, Path.Combine(dirName, typeName + "-appended.csv"));
                }
            }
            else {
                Program.Logger.InfoFormat("Skipping {0}. File does not exist.", fileName);
            }
            return false;
        }

        /// <summary>
        /// Load CSV files in append directory
        /// </summary>
        /// <param name="dirName">Full path name of append directory</param>
        public void LoadAppend(string dirName, DbContextWrapper dbContext) {
            if (Directory.Exists(dirName)) {
                Program.Logger.InfoFormat("Load append files in {0}...", dirName);
                ImportAppendCSV(dirName, "CategorySystem", ImportCategorySystem, dbContext);
                ImportAppendCSV(dirName, "Category", ImportCategory, dbContext);
                ImportAppendCSV(dirName, "Classification", ImportClassification, dbContext);
            }
            else {
                Program.Logger.WarnFormat("Append folder, {0}, does not exist.", dirName);
            }
        }

        /// <summary>
        /// Load CSV files in fragments directory
        /// </summary>
        /// <param name="dirName">Full path name of fragments directory</param>
        public void LoadFragments(string dirName, DbContextWrapper dbContext) {
            if (Directory.Exists(dirName)) {
                Program.Logger.InfoFormat("Load fragment files in {0}...", dirName);
                ImportCSV(Path.Combine( dirName, "Fragment.csv"), CreateFragment, dbContext);
            }
            else {
                Program.Logger.WarnFormat("Fragment folder, {0}, does not exist.", dirName);
            }
        }
    }
}
