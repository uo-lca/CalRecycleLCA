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
            CategorySystem obj = dbContext.CreateEntityWithID<CategorySystem>(Convert.ToInt32(row["CategorySystemID"]));
            if (obj != null) {
                obj.DataTypeID = Convert.ToInt32(row["DataTypeID"]);
                obj.Delimiter = row["Delimiter"];
                obj.Name = row["Name"];
                return (dbContext.SaveChanges() > 0);
            }
            return false;
        }

        private static bool CreateCategory(Row row, DbContextWrapper dbContext) {
            Category obj = dbContext.CreateEntityWithID<Category>(Convert.ToInt32(row["CategoryID"]));
            return (dbContext.SaveChanges() > 0);
        }

        private static bool UpdateCategory(Row row, DbContextWrapper dbContext) {
            int parentID = Convert.ToInt32(row["ParentCategoryID"]);
            Category obj = dbContext.Find<Category>(Convert.ToInt32(row["CategoryID"]));
            if (obj != null) {
                obj.CategorySystemID = Convert.ToInt32(row["CategorySystemID"]);
                obj.ExternalClassID = row["ExternalClassID"];
                obj.HierarchyLevel = Convert.ToInt32(row["HierarchyLevel"]);
                obj.Name = row["Name"];
                if (parentID > 0) {
                    obj.ParentCategoryID = parentID;
                }
                return (dbContext.SaveChanges() > 0);
            }
            return false;
        }

        private static bool ImportClassification(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            string uuid = row["UUID"];
            if (dbContext.IlcdUuidExists(uuid)) {
                Classification obj = dbContext.CreateEntityWithID<Classification>(Convert.ToInt32(row["ClassificationID"])); 
                if (obj != null) {
                    obj.UUID = uuid;
                    obj.CategoryID = Convert.ToInt32(row["CategoryID"]);
                }
                isImported = (dbContext.SaveChanges() > 0);
            }
            else {
                Program.Logger.ErrorFormat("Classification UUID {0} not found. Skipping record.", uuid);
            }
            return isImported;
        }

        private static bool CreateIlcdEntity(string uuid, DataProviderEnum providerEnum, DataTypeEnum typeEnum, DbContextWrapper dbContext) {
            bool uuidExists = false;
            if (dbContext.IlcdUuidExists(uuid)) {
                Program.Logger.WarnFormat("UUID {0} already exists.", uuid);
                uuidExists = true;
            }
            else {
                ILCDEntity ilcdEntity = new ILCDEntity {
                    UUID = uuid,
                    DataProviderID = Convert.ToInt32(providerEnum),
                    DataTypeID = Convert.ToInt32(typeEnum)
                };
                uuidExists = dbContext.AddEntity(ilcdEntity);
            }
            return uuidExists;
        }

        private static bool CreateFragment(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            string uuid = row["FragmentUUID"];
            int fragmentID = Convert.ToInt32(row["FragmentID"]);
            bool uuidExists = CreateIlcdEntity(uuid, DataProviderEnum.fragments, DataTypeEnum.Fragment, dbContext);
            if (uuidExists)
            {
                Fragment obj = dbContext.CreateEntityWithID<Fragment>(fragmentID);
                if (obj != null) obj.UUID = uuid;
                isImported = (dbContext.SaveChanges() > 0);
            }        
            return isImported;
        }

        private static bool UpdateFragment(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int fragmentID = Convert.ToInt32(row["FragmentID"]);
            Fragment ent = dbContext.Find<Fragment>(fragmentID);
            if (ent != null) {
                ent.ReferenceFragmentFlowID = Convert.ToInt32(row["ReferenceFragmentFlowID"]);
                ent.Name = row["Name"];
                if (dbContext.SaveChanges() > 0) isImported = true;
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
            int fragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
            FragmentFlow ent = dbContext.CreateEntityWithID<FragmentFlow>(fragmentFlowID);
            return (dbContext.SaveChanges() > 0);
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
                if (String.IsNullOrEmpty(row["Quantity"]))
                    ent.Quantity = null;
                else
                    ent.Quantity = Convert.ToDouble(row["Quantity"]);
                ent.ParentFragmentFlowID = TransformOptionalID(row["ParentFragmentFlowID"]);
                if (dbContext.SaveChanges() > 0) isImported = true;
            }
            return isImported;
        }

        private static bool ImportFragmentNodeProcess(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int id = Convert.ToInt32(row["FragmentNodeProcessID"]);
            FragmentNodeProcess ent = dbContext.CreateEntityWithID<FragmentNodeProcess>(id);
            if (ent != null) {
                ent.FragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
                ent.ProcessID = Convert.ToInt32(row["ProcessID"]);
                isImported = (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportFragmentNodeFragment(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int id = Convert.ToInt32(row["FragmentNodeFragmentID"]);
            FragmentNodeFragment ent = dbContext.CreateEntityWithID<FragmentNodeFragment>(id);
            if (ent != null) {
                ent.FragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
                ent.SubFragmentID = Convert.ToInt32(row["SubFragmentID"]);
                isImported = (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static IEnumerable<Row> ImportCSV(string fileName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext) {
            int importCounter = 0;
            var table = DataAccess.DataTable.New.ReadCsv(fileName);
            Program.Logger.InfoFormat("Import {0}...", fileName);
            foreach (Row row in table.Rows) {
                if (importRow(row, dbContext)) importCounter++;
            }
            Program.Logger.InfoFormat("{0} of {1} rows imported from {2}.", importCounter, table.Rows.Count(), fileName);
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

        private static IEnumerable<Row> ImportAppendCSV(string dirName, string typeName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext) {
            string fileName = Path.Combine(dirName, typeName + ".csv");
            IEnumerable<Row> rows = null;
            if (System.IO.File.Exists(fileName)) {
                rows = ImportCSV(fileName, importRow, dbContext);
                if (rows.Count() > 0) {
                    System.IO.File.Move(fileName, Path.Combine(dirName, typeName + "-appended.csv"));
                }
            }
            else {
                Program.Logger.InfoFormat("Skipping {0}. File does not exist.", fileName);
            }
            return rows;
        }

        /// <summary>
        /// Load CSV files in append directory
        /// </summary>
        /// <param name="dirName">Full path name of append directory</param>
        public static void LoadAppend(string dirName, DbContextWrapper dbContext) {
            if (Directory.Exists(dirName)) {
                IEnumerable<Row> rows;
                Program.Logger.InfoFormat("Load append files in {0}...", dirName);
                ImportAppendCSV(dirName, "CategorySystem", ImportCategorySystem, dbContext);
                rows = ImportAppendCSV(dirName, "Category", CreateCategory, dbContext);
                if (rows != null) UpdateEntities(rows, UpdateCategory, dbContext);
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
                UpdateEntities(fRows, UpdateFragment, dbContext);
                ImportCSV(Path.Combine(dirName, "FragmentNodeProcess.csv"), ImportFragmentNodeProcess, dbContext);
                ImportCSV(Path.Combine(dirName, "FragmentNodeFragment.csv"), ImportFragmentNodeFragment, dbContext);
            }
            else {
                Program.Logger.WarnFormat("Fragment folder, {0}, does not exist.", dirName);
            }
        }
    }
}
