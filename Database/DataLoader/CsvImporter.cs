﻿using System.Runtime.CompilerServices;
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
            string appendDir = Path.Combine(dirName, "append");
            bool appendDirExists = LoadAppend(appendDir, dbContext);
            LoadFragments(Path.Combine(dirName, "fragments"), dbContext);
            LoadScenarios(Path.Combine(dirName, "scenarios"), dbContext);
            MigrateScenarios(dbContext);
            /*
            if (appendDirExists) {
                LoadClassification(appendDir, dbContext);
            }
             * */
        }

        private static bool ImportCategorySystem(Row row, DbContextWrapper dbContext) {
            bool isNew = true;
            CategorySystem obj = dbContext.ProduceEntityWithID<CategorySystem>(Convert.ToInt32(row["CategorySystemID"]), out isNew);
            if (obj != null) {
                obj.DataTypeID = Convert.ToInt32(row["DataTypeID"]);
                obj.Delimiter = row["Delimiter"];
                obj.Name = row["Name"];
                return (isNew ? AddEntityWithVerification<CategorySystem>(dbContext, obj) : (dbContext.SaveChanges() > 0));
            }
            return false;
        }

        private static bool CreateCategory(Row row, DbContextWrapper dbContext) {
            bool isNew = true;
            Category obj = dbContext.ProduceEntityWithID<Category>(Convert.ToInt32(row["CategoryID"]), out isNew);
            return (isNew ? AddEntityWithVerification<Category>( dbContext, obj) : (dbContext.SaveChanges() > 0));
            
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

        /*
        private static bool ClassificationExists(DbContextWrapper dbContext, int ilcdEntityId, int categoryId)
        {
            return (dbContext.GetDbSet<Classification>().AsQueryable().Where(c => c.ILCDEntityID == ilcdEntityId && c.CategoryID == categoryId)
                .Count() > 0);
        }

        private static bool ImportClassification(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            string uuid = row["UUID"];
            int id = Convert.ToInt32(row["ClassificationID"]);
            ILCDEntity ilcdEntity = dbContext.GetIlcdEntity(uuid);
            if (ilcdEntity == null) {
                Program.Logger.ErrorFormat("Classification UUID {0} not found. Skipping record.", uuid);
            } else {
                int categoryId = Convert.ToInt32(row["CategoryID"]);
                if (ClassificationExists(dbContext, ilcdEntity.ILCDEntityID, categoryId)) {
                    Program.Logger.WarnFormat("Classification ID {0} exists. Skipping record.", id);
                } else {
                    Classification obj = new Classification {
                        ILCDEntityID = ilcdEntity.ILCDEntityID,
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    };
                    isImported = dbContext.AddEntity(obj);
                }
            }
            return isImported;
        }
         * */

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
            if (dbContext.FindRefIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"], out pID, row["ProcessVersion"]) &&
                dbContext.FindRefIlcdEntityID<LcaDataModel.Flow>(row["FlowUUID"], out fID)) {
                DbSet<FlowPropertyEmission> dbSet = dbContext.GetDbSet<FlowPropertyEmission>();
                FlowPropertyEmission fpe = (from o in dbSet
                                   where (o.FlowID == fID)
                                  select o).FirstOrDefault();
                if (fpe == null) {
                    Program.Logger.ErrorFormat("FlowPropertyEmission with FlowID = {0} (UUID {1}) not found.", fID, row["FlowUUID"]);
                }
                else {
                    // how to get EmissionFactor to default to 1 if not supplied?
                    double emissionFactor = Convert.ToDouble(row["EmissionFactor"]);
                    DbSet<ProcessDissipation> pdSet = dbContext.GetDbSet<ProcessDissipation>();
                    ProcessDissipation pd = 
                                      (from o in pdSet
                                       where o.ProcessID == pID
                                       where o.FlowPropertyEmissionID == fpe.FlowPropertyEmissionID
                                       select o).FirstOrDefault();
                    if (pd == null) {
                        pd = new ProcessDissipation { ProcessID = pID, 
                            FlowPropertyEmissionID = fpe.FlowPropertyEmissionID, EmissionFactor = emissionFactor};
                        pdSet.Add(pd);
                    }
                    else {
                        Program.Logger.WarnFormat("ProcessDissipation with ProcessID={0}, FlowID={1} already exists and will be updated.", pID, fID);
                        pd.EmissionFactor = emissionFactor;
                    }
                    isImported = (dbContext.SaveChanges() > 0);
                }                
            }
            return isImported;
        }

        private static bool UpdateDataSource(DataSourceEnum dpEnum, string dirName, DbContextWrapper dbContext) {
            bool updated = false;
            DataSource dp = dbContext.Find<DataSource>(Convert.ToInt32(dpEnum));
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
                        DataSourceID = Convert.ToInt32(DataSourceEnum.fragments),
                        DataTypeID = Convert.ToInt32(DataTypeEnum.Fragment)
                    };
                    dbContext.GetDbSet<ILCDEntity>().Add(ilcdEntity);
                    Fragment obj = new Fragment {
                        FragmentID = fragmentID,
                        ILCDEntity = ilcdEntity
                    };
                    isImported = AddEntityWithVerification<Fragment>(dbContext, obj);
                }
            }
            return isImported;
        }

        private static bool UpdateFragment(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int fragmentID = Convert.ToInt32(row["FragmentID"]);
            Fragment ent = dbContext.Find<Fragment>(fragmentID);
            if (ent != null) {
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
            bool isImported = false, isNew;
            int fragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
            FragmentFlow ent = dbContext.ProduceEntityWithID<FragmentFlow>(fragmentFlowID, out isNew);
            if (ent != null) {
                ent.FragmentFlowID = fragmentFlowID;
                ent.FragmentID = Convert.ToInt32(row["FragmentID"]);
                int flowId;
                if (dbContext.FindRefIlcdEntityID<Flow>(row["FlowUUID"], out flowId))
                    ent.FlowID = flowId;
                else
                    throw new ArgumentNullException("Required field FragmentFlow.FlowID not valid");
                // Set required properties. Others will be set by UpdateFragmentFlow
                ent.DirectionID = Convert.ToInt32(row["DirectionID"]);
                ent.NodeTypeID = Convert.ToInt32(row["NodeTypeID"]);
                ent.FragmentStageID = TransformOptionalID(row["FragmentStageID"]);
                ent.Name = row["Name"];
                ent.ShortName = dbContext.ShortenName(ent.Name, 30);
                isImported = isNew ? AddEntityWithVerification<FragmentFlow>(dbContext, ent) : (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool UpdateFragmentFlow(Row row, DbContextWrapper dbContext) {
            bool isImported = false;
            int fragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
            FragmentFlow ent = dbContext.Find<FragmentFlow>(fragmentFlowID);
            Debug.Assert(ent != null, "FragmentFlow should have been created for this row.");
            if (ent != null) {
                ent.ParentFragmentFlowID = TransformOptionalID(row["ParentFragmentFlowID"]);
                if (dbContext.SaveChanges() > 0) isImported = true;
            }
            return isImported;
        }

        private static bool ImportFragmentStage(Row row, DbContextWrapper dbContext)
        {
            bool isImported = false, isNew;
            int id = Convert.ToInt32(row["FragmentStageID"]);
            FragmentStage ent = dbContext.ProduceEntityWithID<FragmentStage>(id, out isNew);
            if (ent != null)
            {
                ent.Name = row["Name"];
                isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
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
                if (dbContext.FindRefIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"], out refID, row["ProcessVersion"]))
                    ent.ProcessID = refID;
                if (dbContext.FindRefIlcdEntityID<LcaDataModel.Flow>(row["FlowUUID"], out refID))
                    ent.FlowID = refID;
                if (!String.IsNullOrEmpty(row["ConservationFFID"]))
                    ent.ConservationFragmentFlowID = Convert.ToInt32(row["ConservationFFID"]);
                isImported = isNew ? AddEntityWithVerification<FragmentNodeProcess>(dbContext, ent) : (dbContext.SaveChanges() > 0);
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
                ent.Descend = Convert.ToBoolean(Convert.ToInt32(row["Descend"]));
                isImported = isNew ? AddEntityWithVerification<FragmentNodeFragment>(dbContext, ent) : (dbContext.SaveChanges() > 0);
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
                    string version = null;
                    if (NodeTypeEnum.Process.Equals(nodeTypeID)) version= row["ProcessVersion"];
                    ILCDEntity ilcdEntity = dbContext.GetIlcdEntity(row["TargetUUID"], version);
                    if (ilcdEntity == null) {
                        Program.Logger.ErrorFormat("Unable to find ILCDEntity with Background Target UUID, {1} {2}. Skipping record.", row["TargetUUID"], row["ProcessVersion"]);
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

        /// <summary>
        /// Import a row from CompositionModel.csv.
        /// Row is skipped if a Flow with UUID = FlowUUID was not previously loaded.
        /// </summary>
        /// <param name="row">The row to import</param>
        /// <param name="dbContext">Current instance of DbContextWrapper</param>
        /// <returns>true if a CompositionModel record was created or updated, ow false</returns>
        private static bool ImportCompositionModel(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["CompositionModelID"]);
            int refID;
            if (dbContext.FindRefIlcdEntityID<Flow>(row["FlowUUID"], out refID)) {
                CompositionModel ent = dbContext.ProduceEntityWithID<CompositionModel>(id, out isNew);
                ent.FlowID = refID;
                ent.Name = (row["Name"]);
                isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        /// <summary>
        /// Import a row from CompositionData.csv.
        /// Row is skipped if a Flow with UUID = FlowUUID was not previously loaded.
        /// </summary>
        /// <param name="row">The row to import</param>
        /// <param name="dbContext">Current instance of DbContextWrapper</param>
        /// <returns>true if a CompositionData record was created or updated, ow false</returns>
        private static bool ImportCompositionData(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["CompositionDataID"]);
            int refID;
            if (dbContext.FindRefIlcdEntityID<FlowProperty>(row["FlowPropertyUUID"], out refID)) {
                CompositionData ent = dbContext.ProduceEntityWithID<CompositionData>(id, out isNew);
                ent.FlowPropertyID = refID;
                ent.CompositionModelID = Convert.ToInt32(row["CompositionModelID"]);
                ent.Value = Convert.ToDouble(row["Value"]);
                isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }
            
            
        /// <summary>
        /// Import a row from ProcessComposition.csv.
        /// Row is skipped if flow with InFlowUUID does not match CompositionModel flow
        /// </summary>
        /// <param name="row">The row to import</param>
        /// <param name="dbContext">Current instance of DbContextWrapper</param>
        /// <returns>true if a ProcessComposition record was created or updated, ow false</returns>
        private static bool ImportProcessComposition(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["ProcessCompositionID"]),
                modelID = Convert.ToInt32(row["CompositionModelID"]);
            CompositionModel model = dbContext.Find<CompositionModel>(modelID);
            if (model == null) {
                Program.Logger.ErrorFormat("CompositionModelID = {0} was not found. Skipping ProcessComposition {1}.", modelID, id);
            } else {
                int processID, flowID;
                if (dbContext.FindRefIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"], out processID, row["ProcessVersion"]) &&
                    dbContext.FindRefIlcdEntityID<Flow>(row["InFlowUUID"], out flowID)) {
                    if (model.FlowID == flowID) {
                        ProcessComposition ent = dbContext.ProduceEntityWithID<ProcessComposition>(id, out isNew);
                        ent.ProcessID = processID;
                        ent.CompositionModelID = modelID;
                        isImported = isNew ? AddEntityWithVerification<ProcessComposition>(dbContext, ent) : (dbContext.SaveChanges() > 0);
                    }
                    else {
                        Program.Logger.ErrorFormat("InFlowUUID = {0} does not correspond to flow for CompositionModel {1}. Skipping ProcessComposition {2}.", 
                            row["InFlowUUID"], modelID, id);
                    }
                }
            }           
            return isImported;
        }

        private static bool ImportUser(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["UserID"]);
            User ent = dbContext.ProduceEntityWithID<User>(id, out isNew);
            if (ent != null) {
                ent.Name = row["Name"];
                ent.CanLogin = row["CanLogin"].Equals("1");
                ent.CanEditScenarios = row["CanEditScenarios"].Equals("1");
                ent.CanEditFragments = row["CanEditFragments"].Equals("1");
                ent.CanEditBackground = row["CanEditBackground"].Equals("1");
                ent.CanAppend = row["CanAppend"].Equals("1");
                isImported = isNew ? AddEntityWithVerification<User>(dbContext, ent) : (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportScenarioGroup(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["ScenarioGroupID"]);
            ScenarioGroup ent = dbContext.ProduceEntityWithID<ScenarioGroup>(id, out isNew);
            if (ent != null) {
                ent.Name = row["Name"];
                ent.OwnedBy = Convert.ToInt32(row["OwnedBy"]);
                ent.VisibilityID = Convert.ToInt32(row["VisibilityID"]);
                ent.Secret = row["Secret"];
                isImported = isNew ? AddEntityWithVerification<ScenarioGroup>(dbContext, ent) : (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        /// <summary>
        /// Add entity to DbContext and save it. 
        /// Verify that the ID generated by the database matches the ID provided.
        /// InvalidIdException is raised if the IDs do not match.
        /// </summary>
        /// <typeparam name="T">Entity Class</typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <returns>true if the entity was added and saved</returns>
        private static bool AddEntityWithVerification<T>(DbContextWrapper dbContext, T entity) where T : class, IEntity {
            int initialID = entity.ID;
            bool isImported = dbContext.AddEntity(entity);
            if (isImported && entity.ID != initialID) {
                string msg = String.Format(
                    "Entity ID, {0}, does not match database generated ID, {1}.", initialID, entity.ID);
                throw new InvalidIdException(msg);
            }
            return isImported;
        }

        private static int ConvertScenarioID(Row row) {
            return Convert.ToInt32(row["ScenarioID"]) + 1;
        }

        private static bool ImportScenario(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = ConvertScenarioID(row);
            int refID;
            if (dbContext.FindRefIlcdEntityID<Flow>(row["RefFlowUUID"], out refID)) {
                Scenario ent = dbContext.ProduceEntityWithID<Scenario>(id, out isNew);
                ent.Name = row["Name"];
                ent.ScenarioGroupID = Convert.ToInt32(row["ScenarioGroupID"]);
                ent.TopLevelFragmentID = Convert.ToInt32(row["TopLevelFragmentID"]);
                ent.ActivityLevel = Convert.ToDouble(row["ActivityLevel"]);
                ent.FlowID = refID;
                ent.DirectionID = Convert.ToInt32(row["RefDirectionID"]);
                ent.StaleCache = true;
                isImported = isNew ? AddEntityWithVerification<Scenario>( dbContext, ent) : (dbContext.SaveChanges() > 0);
            }
            return isImported;
        }

        private static bool ImportParam(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["ParamID"]);
            Param ent = dbContext.ProduceEntityWithID<Param>(id, out isNew);
            ent.ParamTypeID = Convert.ToInt32(row["ParamTypeID"]);
            ent.ScenarioID = ConvertScenarioID(row);
            ent.Name = row["Name"];
            isImported = isNew ? AddEntityWithVerification<Param>(dbContext, ent) : (dbContext.SaveChanges() > 0);
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

        /* ** get rid of conservation param table
        private static bool ImportConservationParam(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["DependencyParamID"]), fpID;
            ConservationParam ent = dbContext.ProduceEntityWithID<ConservationParam>(id, out isNew);
            ent.FragmentFlowID = Convert.ToInt32(row["FragmentFlowID"]);
            ent.DirectionID = Convert.ToInt32(row["DirectionID"]);
            if (dbContext.FindRefIlcdEntityID<FlowProperty>(row["FlowPropertyUUID"], out fpID))
            {
                ent.FlowPropertyID = fpID;
                isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
            }
            else
            {
                Program.Logger.ErrorFormat("No FlowProperty found with UUID = {0}. Skipping ConservationParam {1}.", 
                    row["FragmentFlowUUID"], id);
                isImported = false;
            }
            return isImported;
        }
         * */

        private static bool ImportCompositionParam(Row row, DbContextWrapper dbContext)
        {
            bool isImported = false, isNew = true;
            int id = Convert.ToInt32(row["CompositionParamID"]);
            CompositionParam ent = dbContext.ProduceEntityWithID<CompositionParam>(id, out isNew);
            ent.ParamID = Convert.ToInt32(row["ParamID"]);
            ent.CompositionDataID = Convert.ToInt32(row["CompositionDataID"]);
            ent.Value = Convert.ToDouble(row["Value"]);
            isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
            return isImported;
        }

        private static bool ImportProcessDissipationParam(Row row, DbContextWrapper dbContext)
        {
            bool isImported = false, isNew = true, progress = false;
            int pID, fID;
            int id = Convert.ToInt32(row["ProcessDissipationParamID"]);
            ProcessDissipationParam ent = dbContext.ProduceEntityWithID<ProcessDissipationParam>(id, out isNew);
            ent.ParamID = Convert.ToInt32(row["ParamID"]);
            progress = dbContext.FindRefIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"], out pID, row["ProcessVersion"]);
            if (progress)
            {
                progress = dbContext.FindRefIlcdEntityID<Flow>(row["FlowUUID"],out fID);
                if (progress)
                {
                    ProcessDissipation PD = dbContext.GetDbSet<ProcessDissipation>()
                        .Where(p => p.ProcessID == pID)
                        .Where(p => p.FlowPropertyEmission.FlowID == fID)
                        .First();
                    ent.ProcessDissipationID = PD.ProcessDissipationID;
                    ent.Value = Convert.ToDouble(row["Value"]);
                    isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
                }
                else
                Program.Logger.ErrorFormat("No Flow found with UUID = {0}. Skipping ProcessDissipationParam {1}.", 
                    row["FlowUUID"], id);
            }
            else
            Program.Logger.ErrorFormat("No Process found with UUID = {0}. Skipping ProcessDissipationParam {1}.", 
                    row["ProcessUUID"], id);

            return isImported;
        }

        private static bool ImportProcessEmissionParam(Row row, DbContextWrapper dbContext)
        {
            bool isImported = false, isNew = true, progress = false;
            int pID, fID;
            int id = Convert.ToInt32(row["ProcessEmissionParamID"]);
            ProcessEmissionParam ent = dbContext.ProduceEntityWithID<ProcessEmissionParam>(id, out isNew);
            ent.ParamID = Convert.ToInt32(row["ParamID"]);
            progress = dbContext.FindRefIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"], out pID, row["ProcessVersion"]);
            if (progress)
            {
                progress = dbContext.FindRefIlcdEntityID<Flow>(row["FlowUUID"],out fID);
                if (progress)
                {
                    ProcessFlow PF = dbContext.GetDbSet<ProcessFlow>()
                        .Where(p => p.ProcessID == pID)
                        .Where(p => p.FlowID == fID)
                        .First();
                    ent.ProcessFlowID = PF.ProcessFlowID;
                    ent.Value = Convert.ToDouble(row["Value"]);
                    isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
                }
                else
                Program.Logger.ErrorFormat("No Flow found with UUID = {0}. Skipping ProcessEmissionParam {1}.", 
                    row["FlowUUID"], id);
            }
            else
            Program.Logger.ErrorFormat("No Process found with UUID = {0}. Skipping ProcessEmissionParam {1}.", 
                    row["ProcessUUID"], id);

            return isImported;
        }

        private static bool ImportCharacterizationParam(Row row, DbContextWrapper dbContext)
        {
            bool isImported = false, isNew = true, progress = false;
            int mID, fID;
            int id = Convert.ToInt32(row["CharacterizationParamID"]);
            CharacterizationParam ent = dbContext.ProduceEntityWithID<CharacterizationParam>(id, out isNew);
            ent.ParamID = Convert.ToInt32(row["ParamID"]);
            progress = dbContext.FindRefIlcdEntityID<LCIAMethod>(row["LCIAMethodUUID"], out mID);
            if (progress)
            {
                progress = dbContext.FindRefIlcdEntityID<Flow>(row["FlowUUID"],out fID);
                if (progress)
                {
                    LCIA L = dbContext.GetDbSet<LCIA>()
                        .Where(p => p.LCIAMethodID == mID)
                        .Where(p => p.FlowID == fID)
                        .First();
                    ent.LCIAID = L.LCIAID;
                    ent.Value = Convert.ToDouble(row["Value"]);
                    isImported = isNew ? dbContext.AddEntity(ent) : (dbContext.SaveChanges() > 0);
                }
                else
                Program.Logger.ErrorFormat("No Flow found with UUID = {0}. Skipping CharacterizationParam {1}.", 
                    row["FlowUUID"], id);
            }
            else
            Program.Logger.ErrorFormat("No LCIA Method found with UUID = {0}. Skipping CharacterizationParam {1}.", 
                    row["LCIAMethodUUID"], id);

            return isImported;
        }


        /// <summary>
        /// Import a row from ProcessSubstitution.csv.
        /// If the ProcessSubstitution table already contains a record with the key values in the row,
        /// this function will attempt to update that record. Otherwise, it will create a new record.
        /// </summary>
        /// <param name="row">Current csv row</param>
        /// <param name="dbContext">Current instance of DbContextWrapper</param>
        /// <returns>true if a ProcessSubstitution record was created or updated, ow false</returns>
        private static bool ImportProcessSubstitution(Row row, DbContextWrapper dbContext) {
            bool isImported = false, isNew = true;
            DbSet<ProcessSubstitution> processSubstitutions = dbContext.GetDbSet<ProcessSubstitution>();
            int refID;
            int [] ids = new int [2];
            ids[0] = Convert.ToInt32(row["FragmentNodeProcessID"]);
            ids[1] = ConvertScenarioID(row);

            ProcessSubstitution ent = processSubstitutions.Find(ids[0], ids[1]);
            if (ent == null) {
                ent = new ProcessSubstitution { FragmentNodeProcessID = ids[0], ScenarioID = ids[1] };
            }
            else {
                isNew = false;
                Program.Logger.WarnFormat("Found ProcessSubstitution with FragmentNodeProcessID = {0}, ScenarioID = {1}.", ids[0], ids[1]);
            }
            if (dbContext.FindRefIlcdEntityID<LcaDataModel.Process>(row["ProcessUUID"], out refID, row["ProcessVersion"]))
                ent.ProcessID = refID;
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

        private static IEnumerable<Row> ImportCSVIfExists(string fileName, Func<Row, DbContextWrapper, bool> importRow, DbContextWrapper dbContext)
        {
            if (File.Exists(fileName))
                return ImportCSV(fileName, importRow, dbContext);
            else
                return null;
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

        /* No need to rename append files
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
         */

        /// <summary>
        /// Load CSV files in append directory
        /// </summary>
        /// <param name="dirName">Full path name of append directory</param>
        public static bool LoadAppend(string dirName, DbContextWrapper dbContext) {
            bool appendExists = Directory.Exists(dirName);
            if (appendExists) {
                IEnumerable<Row> rows;
                Program.Logger.InfoFormat("Load append files in {0}...", dirName);
                UpdateDataSource(DataSourceEnum.append, dirName, dbContext);
                ImportCSV(Path.Combine(dirName, "CategorySystem.csv"), ImportCategorySystem, dbContext);
                rows = ImportCSV(Path.Combine(dirName, "Category.csv"), CreateCategory, dbContext);
                if (rows != null) UpdateEntities(rows, UpdateCategory, dbContext);
                ImportCSV(Path.Combine(dirName, "FlowPropertyEmission.csv"), ImportFlowPropertyEmission, dbContext);
                ImportCSV(Path.Combine(dirName, "ProcessDissipation.csv"), ImportProcessDissipation, dbContext);
                ImportCSV(Path.Combine(dirName, "CompositionModel.csv"), ImportCompositionModel, dbContext);
                ImportCSV(Path.Combine(dirName, "CompositionData.csv"), ImportCompositionData, dbContext);
                ImportCSV(Path.Combine(dirName, "ProcessComposition.csv"), ImportProcessComposition, dbContext);
                // Defer loading Classification until the end.
            }
            else {
                Program.Logger.WarnFormat("Append folder, {0}, does not exist.", dirName);
            }
            return appendExists;
        }

        /*
        /// <summary>
        /// Import Classification last because it references UUIDs in other files and will end with large DbSet.
        ///  Improve performance by disabling AutoDetectChanges and only executing Adds (no updates).
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="dbContext"></param>
        public static void LoadClassification(string dirName, DbContextWrapper dbContext) {
            dbContext.SetAutoDetectChanges(false);
            ImportCSV(Path.Combine(dirName, "Classification.csv"), ImportClassification, dbContext);
            dbContext.SetAutoDetectChanges(true);
        }
         * */

        /// <summary>
        /// Load CSV files in fragments directory
        /// </summary>
        /// <param name="dirName">Full path name of fragments directory</param>
        /// <param name="dbContext">Object containing current DbContext</param>
        public static void LoadFragments(string dirName, DbContextWrapper dbContext) {
            if (Directory.Exists(dirName)) {
                IEnumerable<Row> fRows, ffRows;
                Program.Logger.InfoFormat("Load fragment files in {0}...", dirName);
                UpdateDataSource(DataSourceEnum.fragments, dirName, dbContext);
                fRows = ImportCSV(Path.Combine(dirName, "Fragment.csv"), CreateFragment, dbContext);
                ImportCSV(Path.Combine(dirName, "FragmentStage.csv"), ImportFragmentStage, dbContext);
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
                UpdateDataSource(DataSourceEnum.scenarios, dirName, dbContext);
                ImportCSV(Path.Combine(dirName, "User.csv"), ImportUser, dbContext);
                ImportCSV(Path.Combine(dirName, "ScenarioGroup.csv"), ImportScenarioGroup, dbContext);
                ImportCSV(Path.Combine(dirName, "Scenario.csv"), ImportScenario, dbContext);
                ImportCSVIfExists(Path.Combine(dirName, "ProcessSubstitution.csv"), ImportProcessSubstitution, dbContext);
                if (ImportCSVIfExists(Path.Combine(dirName, "Param.csv"), ImportParam, dbContext) == null)
                    return;
                ImportCSVIfExists(Path.Combine(dirName, "DependencyParam.csv"), ImportDependencyParam, dbContext);
                //ImportCSVIfExists(Path.Combine(dirName, "ConservationParam.csv"), ImportConservationParam, dbContext);
                //ImportCSV(Path.Combine(dirName, "FlowPropertyParam.csv"), ImportFlowPropertyParam, dbContext);
                ImportCSVIfExists(Path.Combine(dirName, "CompositionParam.csv"), ImportCompositionParam, dbContext);
                ImportCSVIfExists(Path.Combine(dirName, "ProcessDissipationParam.csv"), ImportProcessDissipationParam, dbContext);
                ImportCSVIfExists(Path.Combine(dirName, "ProcessEmissionParam.csv"), ImportProcessEmissionParam, dbContext);
                ImportCSVIfExists(Path.Combine(dirName, "CharacterizationParam.csv"), ImportCharacterizationParam, dbContext);
            }
            else {
                Program.Logger.WarnFormat("Scenarios folder, {0}, does not exist.", dirName);
            }
        }
        public static void MigrateScenarios(DbContextWrapper dbContext)
        {
            // this function sets all scenario reference flows to the reference flow of the top level fragment.
            // note: this could result in very large activity levels
            var scenarios = dbContext.GetDbSet<Scenario>();
            foreach (Scenario scenario in scenarios)
            {
                var refFlow = dbContext.FragmentReferenceFlow(scenario.TopLevelFragmentID);
                scenario.FlowID = refFlow.FlowID;
                scenario.DirectionID = Direction.comp(refFlow.DirectionID);
            }
            // clear the caches -- must run /config/init afterwards
            var nodeCaches = dbContext.GetDbSet<NodeCache>();
            nodeCaches.RemoveRange(nodeCaches);
            var scoreCaches = dbContext.GetDbSet<ScoreCache>();
            scoreCaches.RemoveRange(scoreCaches);
            dbContext.SaveChanges();
        }
    }
}
