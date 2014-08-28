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
        /// <param name="dbContext">Shared instance of DbContextWrapper</param>
        public static void LoadAll(string dirName, DbContextWrapper dbContext) {
            LoadAppend(Path.Combine(dirName, "append"), dbContext);
            LoadFragments(Path.Combine(dirName, "fragments"), dbContext);
            LoadScenarios(Path.Combine(dirName, "scenarios"), dbContext);
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
            int id = Convert.ToInt32(row["ClassificationID"]);
            ILCDEntity ilcdEntity = dbContext.GetIlcdEntity(uuid);
            if (ilcdEntity == null) {
                Program.Logger.ErrorFormat("Classification UUID {0} not found. Skipping record.", uuid);
            } else {
                if (dbContext.EntityIdExists<Classification>(id)) {
                    Program.Logger.WarnFormat("Classification ID {0} exists. Skipping record.", id);
                } else {
                    Classification obj = new Classification {
                        ClassificationID = id,
                        ILCDEntityID = ilcdEntity.ILCDEntityID,
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    };
                    isImported = dbContext.AddEntity(obj);
                }
            }
            return isImported;
        }

        private static bool ImportFlowPropertyEmission(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int fpID, fID;
            if (dbContext.FindRefIlcdEntityID<FlowProperty>(row["FlowPropertyUUID"], out fpID) &&
                dbContext.FindRefIlcdEntityID<Flow>(row["EmissionUUID"], out fID)) {
                double scale = Convert.ToDouble(row["Scale"]);
                DbSet<FlowPropertyEmission> fpEmissions = dbContext.GetDbSet<FlowPropertyEmission>();
                FlowPropertyEmission fpEmission = (from fpe in fpEmissions 
                                                   where fpe.FlowPropertyID == fpID && fpe.FlowID == fID
                                                   select fpe).FirstOrDefault();
                if (fpEmission == null) {
                    fpEmission = new FlowPropertyEmission { FlowPropertyID = fpID, FlowID = fID, Scale = scale };
                    fpEmissions.Add(fpEmission);
                }
                else {
                    Program.Logger.WarnFormat("FlowPropertyEmission with FlowPropertyID={0} and FlowID={1} already exists and will be updated.", fpID, fID);
                    fpEmission.Scale = scale;
                }
                
                isImported = (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportProcessDissipation(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int pID, fID;
            int dirID = Convert.ToInt32(DirectionEnum.Output);
            if (dbContext.FindRefIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"], out pID) &&
                dbContext.FindRefIlcdEntityID<LcaDataModel.Flow>(row["FlowUUID"], out fID)) {
                DbSet<ProcessFlow> dbSet = dbContext.GetDbSet<ProcessFlow>();
                ProcessFlow obj = (from o in dbSet
                                   where (o.ProcessID == pID) && (o.FlowID == fID) && (o.DirectionID == dirID)
                                  select o).FirstOrDefault();
                if (obj == null) {
                    Program.Logger.ErrorFormat("ProcessFlow with ProcessID = {0}, FlowID = {1}, and Output direction not found.", pID, fID);
                }
                else {
                    double emissionFactor = Convert.ToDouble(row["EmissionFactor"]);
                    DbSet<ProcessDissipation> pdSet = dbContext.GetDbSet<ProcessDissipation>();
                    ProcessDissipation pd = 
                                      (from o in pdSet
                                       where o.ProcessFlowID == obj.ProcessFlowID
                                       select o).FirstOrDefault();
                    if (pd == null) {
                        pd = new ProcessDissipation { ProcessFlowID = obj.ProcessFlowID, EmissionFactor = emissionFactor};
                        pdSet.Add(pd);
                    }
                    else {
                        Program.Logger.WarnFormat("ProcessDissipation with ProcessFlowID={0} already exists and will be updated.", obj.ProcessFlowID);
                        pd.EmissionFactor = emissionFactor;
                    }
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

        /// <summary>
        /// Create Fragment record for row.
        /// Fragment is like other ILCD data types in that it has a UUID.
        /// However, it also has an external ID (FragmentID), created in MatLab, and 
        /// FragmentID is used to reference fragments in other data sources. Therefore, this method
        /// does not use IlcdEntity methods that optimize UUID -> ID lookup.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private static bool CreateFragment(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            string uuid = row["FragmentUUID"];
            int fragmentID = Convert.ToInt32(row["FragmentID"]);
            if (!dbContext.EntityIdExists<Fragment>(fragmentID)) {
                if (!dbContext.IlcdEntityAlreadyExists<Fragment>(uuid)) {
                    ILCDEntity ilcdEntity = new ILCDEntity {
                        UUID = uuid,
                        DataProviderID = Convert.ToInt32(DataProviderEnum.fragments),
                        DataTypeID = Convert.ToInt32(DataTypeEnum.Fragment)
                    };
                    dbContext.GetDbSet<ILCDEntity>().Add(ilcdEntity);
                    Fragment obj = new Fragment {
                        FragmentID = fragmentID,
                        ILCDEntity = ilcdEntity
                    };
                    isImported = dbContext.AddEntity(obj);
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
            if (dbContext.Find<FragmentFlow>(fragmentFlowID) == null) {
                return dbContext.AddEntity(new FragmentFlow { FragmentFlowID = fragmentFlowID });
            }
            else {
                Program.Logger.WarnFormat("Found FragmentFlow with ID = {0}. Entity will not be added, but may be updated.", fragmentFlowID);
                return false;
            }
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
                ent.ShortName = dbContext.ShortenName(ent.Name, 30);
                ent.NodeTypeID = Convert.ToInt32(row["NodeTypeID"]);
                if (!string.IsNullOrEmpty(row["FlowUUID"])) {
                    int flowID;
                    if (dbContext.FindRefIlcdEntityID<Flow>(row["FlowUUID"], out flowID))
                        ent.FlowID = flowID;
                }
                ent.DirectionID = Convert.ToInt32(row["DirectionID"]);
                ent.ParentFragmentFlowID = TransformOptionalID(row["ParentFragmentFlowID"]);
                if (dbContext.SaveChanges() > 0) isImported = true;
            }
            return isImported;
        }

        private static bool ImportFragmentNodeProcess(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew;
            int id = Convert.ToInt32(row["FragmentNodeProcessID"]);
            FragmentNodeProcess ent = dbContext.ProduceEntityWithID<FragmentNodeProcess>(id, out isNew);
            if (ent != null) {
                int refID;
                ent.FragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
                if (dbContext.FindRefIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"], out refID))
                    ent.ProcessID = refID;
                if (dbContext.FindRefIlcdEntityID<LcaDataModel.Flow>(row["FlowUUID"], out refID))
                    ent.FlowID = refID;
                isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportFragmentNodeFragment(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew;
            int id = Convert.ToInt32(row["FragmentNodeFragmentID"]);
            FragmentNodeFragment ent = dbContext.ProduceEntityWithID<FragmentNodeFragment>(id, out isNew);
            if (ent != null) {
                int refID;
                ent.FragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
                ent.SubFragmentID = Convert.ToInt32(row["SubFragmentID"]);
                if (dbContext.FindRefIlcdEntityID<LcaDataModel.Flow>(row["FlowUUID"], out refID))
                    ent.FlowID = refID;
                isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        /// <summary>
        /// Import a row from Background.csv.
        /// Row is skipped if the following errors are detected:
        ///     A Flow with UUID = FlowUUID was not previously loaded.
        ///     The row has a TargetUUID, but the ILCDEntityID table does not contain that UUID.
        /// </summary>
        /// <param name="row">The row to import</param>
        /// <param name="dbContext">Current instance of DbContextWrapper</param>
        /// <returns>true if a Background record was created or updated, ow false</returns>
        private static bool ImportBackground(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["BackgroundID"]);
            int refID;
            if (dbContext.FindRefIlcdEntityID<Flow>(row["FlowUUID"], out refID)) {
                int? ilcdEntityID = null;
                int nodeTypeID = Convert.ToInt32(row["NodeTypeID"]);
                if (!string.IsNullOrEmpty(row["TargetUUID"])) {
                    ILCDEntity ilcdEntity = dbContext.GetIlcdEntity(row["TargetUUID"]);
                    if (ilcdEntity == null) {
                        Program.Logger.ErrorFormat("Unable to find ILCDEntity with Background Target UUID, {1}. Skipping record.", row["TargetUUID"]);
                    } else {
                        ilcdEntityID = ilcdEntity.ILCDEntityID;
                    }
                }
                if (ilcdEntityID != null || nodeTypeID == 5 ) {
                    Background ent = dbContext.ProduceEntityWithID<Background>(id, out isNew);
                    ent.NodeTypeID = nodeTypeID;
                    ent.FlowID = refID;
                    ent.DirectionID = Convert.ToInt32(row["DirectionID"]);
                    ent.ILCDEntityID = ilcdEntityID;
                    isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
                }
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

        private static bool ImportParam(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["ParamID"]);
            Param ent = dbContext.ProduceEntityWithID<Param>(id, out isNew);
            ent.ParamTypeID = Convert.ToInt32(row["ParamTypeID"]);
            ent.ScenarioID = Convert.ToInt32(row["ScenarioID"]);
            ent.Name = row["Name"];
            isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
            return isImported;
        }

        private static bool ImportDependencyParam(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["DependencyParamID"]);
            DependencyParam ent = dbContext.ProduceEntityWithID<DependencyParam>(id, out isNew);
            ent.ParamID = Convert.ToInt32(row["ParamID"]);
            ent.FragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
            ent.Value = Convert.ToDouble(row["Value"]);
            isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
            return isImported;
        }

        private static bool ImportDistributionParam(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["DependencyParamID"]);
            DistributionParam ent = dbContext.ProduceEntityWithID<DistributionParam>(id, out isNew);
            ent.ConservationDependencyParamID = Convert.ToInt32(row["ConservationDependencyParamID"]);
            isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
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
                ImportAppendCSV(dirName, "FlowPropertyEmission", ImportFlowPropertyEmission, dbContext);
                ImportAppendCSV(dirName, "ProcessDissipation", ImportProcessDissipation, dbContext);
                // Import Classification last because it references UUIDs in other files
                // Improve performance by disabling AutoDetectChanges and only executing Adds (no updates).
                dbContext.SetAutoDetectChanges(false);
                ImportAppendCSV(dirName, "Classification", ImportClassification, dbContext);
                dbContext.SetAutoDetectChanges(true);
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
                ImportCSV(Path.Combine(dirName, "Background.csv"), ImportBackground, dbContext);
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
                ImportCSV(Path.Combine(dirName, "Param.csv"), ImportParam, dbContext);
                //ImportCSV(Path.Combine(dirName, "ScenarioParam.csv"), ImportScenarioParam, dbContext);
                ImportCSV(Path.Combine(dirName, "DependencyParam.csv"), ImportDependencyParam, dbContext);
                ImportCSV(Path.Combine(dirName, "DistributionParam.csv"), ImportDistributionParam, dbContext);
            }
            else {
                Program.Logger.WarnFormat("Scenarios folder, {0}, does not exist.", dirName);
            }
        }
    }
}
