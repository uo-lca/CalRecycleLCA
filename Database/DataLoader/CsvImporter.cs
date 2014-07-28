using System.Runtime.CompilerServices;
using System;
using System.Data.Entity;
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
                LoadScenarios(Path.Combine(dirName, "scenarios"), dbContext);
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
            ILCDEntity ilcdEntity = dbContext.GetIlcdEntity(uuid);
            if (dbContext.IlcdUuidExists(uuid)) {
                Classification obj = dbContext.CreateEntityWithID<Classification>(Convert.ToInt32(row["ClassificationID"])); 
                if (obj != null) {
                    obj.ILCDEntity = ilcdEntity;
                    obj.CategoryID = Convert.ToInt32(row["CategoryID"]);
                }
                isImported = (dbContext.SaveChanges() > 0);
            }
            else {
                Program.Logger.ErrorFormat("Classification UUID {0} not found. Skipping record.", uuid);
            }
            return isImported;
        }

        private static bool ImportFlowPropertyEmission(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int? fpID = dbContext.GetIlcdEntityID<FlowProperty>(row["FlowPropertyUUID"]);
            int? fID = dbContext.GetIlcdEntityID<Flow>(row["EmissionUUID"]);
            if (fpID != null && fID != null) {
                DbSet<FlowPropertyEmission> fpEmissions = dbContext.GetDbSet<FlowPropertyEmission>();
                FlowPropertyEmission fpEmission = (from fpe in fpEmissions 
                                                   where fpe.FlowPropertyID == fpID && fpe.FlowID == fID
                                                   select fpe).FirstOrDefault();
                if (fpEmission == null) {
                    fpEmission = new FlowPropertyEmission { FlowPropertyID = fpID, FlowID = fID };
                    fpEmissions.Add(fpEmission);
                }
                else {
                    Program.Logger.WarnFormat("FlowPropertyEmission with FlowPropertyID={0} and FlowID={1} already exists and will be updated.", fpID, fID);
                }
                fpEmission.Scale = Convert.ToDouble(row["Scale"]);
                isImported = (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportFlowFlowProperty(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int? pID = dbContext.GetIlcdEntityID<LcaDataModel.FlowProperty>(row["FlowPropertyUUID"]);
            int? fID = dbContext.GetIlcdEntityID<Flow>(row["FlowUUID"]);
            if (pID != null && fID != null) {
                int fpID = Convert.ToInt32(pID);
                int flowID = Convert.ToInt32(fID);
                DbSet<FlowFlowProperty> dbSet = dbContext.GetDbSet<FlowFlowProperty>();
                FlowFlowProperty obj = (from o in dbSet
                                   where (o.FlowPropertyID == fpID) && (o.FlowID == flowID)
                                   select o).FirstOrDefault();
                if (obj == null) {
                    obj = new FlowFlowProperty { FlowID = flowID, FlowPropertyID = fpID };
                    dbSet.Add(obj);
                }
                else {
                    Program.Logger.WarnFormat("FlowFlowProperty with FlowID={0} and FlowPropertyID={1} already exists and will be updated.", obj.FlowID, obj.FlowPropertyID);
                }
                obj.MeanValue = Convert.ToDouble(row["MeanValue"]);
                obj.StDev = Convert.ToDouble(row["StDev"]);
                isImported = (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportProcessDissipation(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int? pID = dbContext.GetIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"]);
            int? fID = dbContext.GetIlcdEntityID<Flow>(row["FlowUUID"]);
            int dirID = Convert.ToInt32(DirectionEnum.Output);
            if (pID != null && fID != null) {
                DbSet<ProcessFlow> dbSet = dbContext.GetDbSet<ProcessFlow>();
                ProcessFlow obj = (from o in dbSet
                                   where (o.ProcessID == pID) && (o.FlowID == fID) && (o.DirectionID == dirID)
                                  select o).FirstOrDefault();
                if (obj == null) {
                    Program.Logger.ErrorFormat("ProcessFlow with ProcessID = {0}, FlowID = {1}, and Output direction not found.", pID, fID);
                }
                else {
                    DbSet<ProcessDissipation> pdSet = dbContext.GetDbSet<ProcessDissipation>();
                    ProcessDissipation pd = 
                                      (from o in pdSet
                                       where o.ProcessFlowID == obj.ProcessFlowID
                                       select o).FirstOrDefault();
                    if (pd == null) {
                        pd = new ProcessDissipation { ProcessFlowID = obj.ProcessFlowID };
                        pdSet.Add(pd);
                    }
                    else {
                        Program.Logger.WarnFormat("ProcessDissipation with ProcessFlowID={0} already exists and will be updated.", obj.ProcessFlowID);
                    }
                    pd.EmissionFactor = Convert.ToDouble(row["EmissionFactor"]);
                    isImported = (dbContext.SaveChanges() > 0);
                }                
            }
            return isImported;
        }

        private static bool UpdateDataProvider(DataProviderEnum dpEnum, string dirName, DbContextWrapper dbContext) {
            bool updated = false;
            DataProvider dp = dbContext.Find<DataProvider>(Convert.ToInt32(dpEnum));
            if (dp == null) {
                Program.Logger.ErrorFormat("Data provider for {0} not found.", dpEnum.ToString());
            }
            else {
                dp.DirName = dirName;
                updated = (dbContext.SaveChanges() > 0);
            }
            return updated;
        }

        private static ILCDEntity CreateIlcdEntity(string uuid, DataProviderEnum providerEnum, DataTypeEnum typeEnum, DbContextWrapper dbContext) {
            ILCDEntity ilcdEntity = null;
            if (dbContext.IlcdUuidExists(uuid)) {
                Program.Logger.ErrorFormat("UUID {0} already exists. ILCDEntity for {0}, {1} will not be added.", uuid, 
                    Convert.ToString(providerEnum), Convert.ToString(typeEnum));
            }
            else {
                ilcdEntity = new ILCDEntity {
                    UUID = uuid,
                    DataProviderID = Convert.ToInt32(providerEnum),
                    DataTypeID = Convert.ToInt32(typeEnum)                  
                };
                ilcdEntity = dbContext.GetDbSet<ILCDEntity>().Add(ilcdEntity);
            }
            return ilcdEntity;
        }

        private static bool ImportFlow(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            Flow flow = dbContext.GetIlcdEntity<Flow>(row["FlowUUID"]);
            int? refID = dbContext.GetIlcdEntityID<FlowProperty>(row["ReferenceFlowPropertyUUID"]);
            if (refID == null) {
                Program.Logger.ErrorFormat("ReferenceFlowPropertyUUID not found. Flow {1} will not be imported.", row["FlowUUID"]);
            }
            else {
                if (flow == null) {
                    ILCDEntity ilcdEntity = CreateIlcdEntity(row["FlowUUID"], DataProviderEnum.append, DataTypeEnum.Flow, dbContext);
                    if (ilcdEntity != null) {
                        flow = new Flow();
                        flow.ILCDEntity = ilcdEntity;
                        dbContext.GetDbSet<Flow>().Add(flow);
                    }
                }
                else {
                    Program.Logger.WarnFormat("Flow with UUID {0} already exists. Flow will be updated.", row["FlowUUID"]);
                }
                if (flow != null) {
                    flow.CASNumber = row["CASNumber"];
                    flow.FlowTypeID = Convert.ToInt32(row["FlowTypeID"]);
                    flow.ReferenceFlowProperty = refID;
                    isImported = (dbContext.SaveChanges() > 0);
                }
            }
            return isImported;
        }

        private static bool ImportFlowProperty(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            FlowProperty flowProperty = dbContext.GetIlcdEntity<FlowProperty>(row["FlowPropertyUUID"]);
            int? ugID = dbContext.GetIlcdEntityID<UnitGroup>(row["UnitGroupUUID"]);
            if (ugID == null) {
                Program.Logger.ErrorFormat("UnitGroupUUID not found. FlowProperty {1} will not be imported.", row["FlowPropertyUUID"]);
            }
            else {
                if (flowProperty == null) {
                    ILCDEntity ilcdEntity = CreateIlcdEntity(row["FlowPropertyUUID"], DataProviderEnum.append, DataTypeEnum.FlowProperty, dbContext);
                    if (ilcdEntity != null) {
                        flowProperty = new FlowProperty();
                        flowProperty.ILCDEntity = ilcdEntity;
                        dbContext.GetDbSet<FlowProperty>().Add(flowProperty);
                    }
                }
                else {
                    Program.Logger.WarnFormat("FlowProperty with UUID {0} already exists. FlowProperty will be updated.", row["FlowPropertyUUID"]);
                }
                if (flowProperty != null) {
                    flowProperty.Name = row["Name"];
                    flowProperty.UnitGroupID = ugID;
                    isImported = (dbContext.SaveChanges() > 0);
                }
            }
            return isImported;
        }

        private static bool CreateFragment(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            string uuid = row["FragmentUUID"];
            int fragmentID = Convert.ToInt32(row["FragmentID"]);
            if (!dbContext.EntityIdExists<Fragment>(fragmentID)) {
                ILCDEntity ilcdEntity = CreateIlcdEntity(uuid, DataProviderEnum.fragments, DataTypeEnum.Fragment, dbContext);
                if (ilcdEntity != null) {
                    Fragment obj = dbContext.CreateEntityWithID<Fragment>(fragmentID);
                    if (obj != null) {
                        obj.ILCDEntity = ilcdEntity;
                    }
                    isImported = (dbContext.SaveChanges() > 0);
                }
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
                ent.NodeTypeID = Convert.ToInt32(row["NodeTypeID"]);
                ent.FlowID = dbContext.GetIlcdEntityID<Flow>(row["FlowUUID"]);
                ent.DirectionID = Convert.ToInt32(row["DirectionID"]);
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
                ent.ProcessID =  dbContext.GetIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"]);
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

        private static bool ImportUser(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int id = Convert.ToInt32(row["UserID"]);
            User ent = dbContext.CreateEntityWithID<User>(id);
            if (ent != null) {
                ent.Name = row["Name"];
                ent.CanLogin = row["CanLogin"].Equals("1");
                ent.CanEditScenarios = row["CanEditScenarios"].Equals("1");
                ent.CanEditFragments = row["CanEditFragments"].Equals("1");
                ent.CanEditBackground = row["CanEditBackground"].Equals("1");
                ent.CanAppend = row["CanAppend"].Equals("1");
                isImported = (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportScenarioGroup(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int id = Convert.ToInt32(row["ScenarioGroupID"]);
            ScenarioGroup ent = dbContext.CreateEntityWithID<ScenarioGroup>(id);
            if (ent != null) {
                ent.Name = row["Name"];
                ent.OwnedBy = Convert.ToInt32(row["OwnedBy"]);
                ent.VisibilityID = Convert.ToInt32(row["VisibilityID"]);
                isImported = (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportScenario(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int id = Convert.ToInt32(row["ScenarioID"]);
            Scenario ent = dbContext.CreateEntityWithID<Scenario>(id);
            if (ent != null) {
                ent.Name = row["Name"];
                ent.ScenarioGroupID = Convert.ToInt32(row["ScenarioGroupID"]);
                isImported = (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static IEnumerable<Row> ImportCSV(string fileName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext) {
            int importCounter = 0;
            var table = DataAccess.DataTable.New.ReadCsv(fileName);
            Program.Logger.InfoFormat("Import {0}...", fileName);
            foreach (Row row in table.Rows) {
                Program.Logger.DebugFormat("Import row {0}", row.DebugValues);
                if (importRow(row, dbContext)) importCounter++;
            }
            Program.Logger.InfoFormat("{0} of {1} rows imported from {2}.", importCounter, table.Rows.Count(), fileName);
            return table.Rows;
        }

        private static int UpdateEntities(IEnumerable<Row> rows, Func<Row, DbContextWrapper, bool> updateRow, DbContextWrapper dbContext) {
            int importCounter = 0;
            Program.Logger.InfoFormat("Update imported rows...");
            foreach (Row row in rows) {
                Program.Logger.DebugFormat("Update row {0}", row.DebugValues);
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
                UpdateDataProvider(DataProviderEnum.append, dirName, dbContext);
                ImportAppendCSV(dirName, "CategorySystem", ImportCategorySystem, dbContext);
                rows = ImportAppendCSV(dirName, "Category", CreateCategory, dbContext);
                if (rows != null) UpdateEntities(rows, UpdateCategory, dbContext);
                ImportAppendCSV(dirName, "Flow-append", ImportFlow, dbContext);
                ImportAppendCSV(dirName, "FlowProperty-append", ImportFlowProperty, dbContext);
                ImportAppendCSV(dirName, "FlowFlowProperty-append", ImportFlowFlowProperty, dbContext);
                ImportAppendCSV(dirName, "FlowPropertyEmission", ImportFlowPropertyEmission, dbContext);
                ImportAppendCSV(dirName, "ProcessDissipation", ImportProcessDissipation, dbContext);
                // Import Classification last because it references UUIDs in other files
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
                UpdateDataProvider(DataProviderEnum.fragments, dirName, dbContext);
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

        /// <summary>
        /// Load CSV files in scenarios directory
        /// </summary>
        /// <param name="dirName">Full path name of scenarios directory</param>
        /// <param name="dbContext">Object containing current DbContext</param>
        public static void LoadScenarios(string dirName, DbContextWrapper dbContext) {
            if (Directory.Exists(dirName)) {
                Program.Logger.InfoFormat("Load scenario files in {0}...", dirName);
                UpdateDataProvider(DataProviderEnum.scenarios, dirName, dbContext);
                ImportCSV(Path.Combine(dirName, "User.csv"), ImportUser, dbContext);
                ImportCSV(Path.Combine(dirName, "ScenarioGroup.csv"), ImportScenarioGroup, dbContext);
                ImportCSV(Path.Combine(dirName, "Scenario.csv"), ImportScenario, dbContext);
            }
            else {
                Program.Logger.WarnFormat("Scenarios folder, {0}, does not exist.", dirName);
            }
        }
    }
}
