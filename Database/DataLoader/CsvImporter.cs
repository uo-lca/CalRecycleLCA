using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;
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
        public static void LoadAll(string dirName) {
            using (DbContextWrapper dbContext = new DbContextWrapper()) {
                LoadAppend(Path.Combine(dirName, "append"), dbContext);
                LoadFragments(Path.Combine(dirName, "fragments"), dbContext);
            }
        }

        private static bool ImportCategorySystem(Row row, DbContextWrapper dbContext) {
            CategorySystem obj = new CategorySystem {
                DataTypeID = Convert.ToInt32(row["DataTypeID"]),
                Delimiter = row["Delimiter"],
                Name = row["Name"]
            } ;   
            return dbContext.AddEntity(obj);
        }

        private static bool ImportCategory(Row row, DbContextWrapper dbContext) {
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

        private static bool ImportClassification(Row row, DbContextWrapper dbContext) {
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

        private static bool CreateIlcdEntity(string uuid, DataProviderEnum providerEnum, DataTypeEnum typeEnum, DbContextWrapper dbContext) {
            ILCDEntity ilcdEntity = new ILCDEntity {
                UUID = uuid,
                DataProviderID = Convert.ToInt32(providerEnum),
                DataTypeID = Convert.ToInt32(typeEnum)
            };
            return dbContext.AddEntity(ilcdEntity);
        }

        private static bool CreateFragment(Row row, DbContextWrapper dbContext) {
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

        public static int? TransformOptionalID(string idString) {
            int? id = null;
            if (!String.IsNullOrEmpty(idString)) {
                id = Convert.ToInt32(idString);
                if (id == 0) id = null;
            }
            return id;
        }

        private static bool CreateFragmentFlow(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int fragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
            if (dbContext.EntityIdExists<FragmentFlow>(fragmentFlowID)) {
                Program.Logger.WarnFormat("FragmentFlowID, {0}, already exists.", row["FragmentFlowID"]);
            }
            else {
                FragmentFlow ent = new FragmentFlow {
                    FragmentFlowID = fragmentFlowID
                };
                isImported = dbContext.AddEntity<FragmentFlow>(ent);
            }
            return isImported;
        }

        private static bool UpdateFragmentFlow(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int fragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
            FragmentFlow ent = dbContext.Find<FragmentFlow>(fragmentFlowID);
            Debug.Assert(ent != null, "FragmentFlow should have been created for this row.");
            if (ent != null) {
                ent.FragmentID = Convert.ToInt32(row["FragmentID"]);
                ent.FragmentStageID = TransformOptionalID(row["FragmentStageID"]);
                ent.Name = row["Name"];
                ent.ReferenceFlowPropertyID = dbContext.GetIlcdEntityID<FlowProperty>(row["ReferenceFlowPropertyUUID"]);
                ent.NodeTypeID = Convert.ToInt32(row["NodeTypeID"]);
                ent.FlowID = dbContext.GetIlcdEntityID<Flow>(row["FlowUUID"]);
                ent.DirectionID = Convert.ToInt32(row["DirectionID"]);
                ent.ParentFragmentFlowID = TransformOptionalID(row["ParentFragmentFlowID"]);
                if (dbContext.SaveChanges() > 0) isImported = true;
            }
            return isImported;
        }

        private static IEnumerable<Row> ImportCSV(string fileName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext) {
            int importCounter = 0;
            var table = DataAccess.DataTable.New.ReadCsv(fileName);
            foreach (Row row in table.Rows) {
                if (importRow(row, dbContext)) importCounter++;
            }
            Program.Logger.InfoFormat("{0} of {1} records imported from {2}.", importCounter, table.Rows.Count(), fileName);
            return table.Rows;
        }

        private static int UpdateEntities(IEnumerable<Row> rows, Func<Row, DbContextWrapper, bool> updateRow, DbContextWrapper dbContext) {
            int importCounter = 0;
            foreach (Row row in rows) {
                if (updateRow(row, dbContext)) importCounter++;
            }
            Program.Logger.InfoFormat("Updated {0} of {1} entities.", importCounter, rows.Count());
            return importCounter;
        }

        private static bool ImportAppendCSV(string dirName, string typeName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext) {
            string fileName = Path.Combine(dirName, typeName + ".csv");
            if (System.IO.File.Exists(fileName)) {
                if (ImportCSV(fileName, importRow, dbContext).Count() > 0) {
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
        public static void LoadAppend(string dirName, DbContextWrapper dbContext) {
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
        /// <param name="dbContext">Object containing current DbContext</param>
        public static void LoadFragments(string dirName, DbContextWrapper dbContext) {
            if (Directory.Exists(dirName)) {
                IEnumerable<Row> fRows, ffRows;
                Program.Logger.InfoFormat("Load fragment files in {0}...", dirName);
                fRows = ImportCSV(Path.Combine(dirName, "Fragment.csv"), CreateFragment, dbContext);
                ffRows = ImportCSV(Path.Combine(dirName, "FragmentFlow.csv"), CreateFragmentFlow, dbContext);
                UpdateEntities(ffRows, UpdateFragmentFlow, dbContext);
            }
            else {
                Program.Logger.WarnFormat("Fragment folder, {0}, does not exist.", dirName);
            }
        }
    }
}
