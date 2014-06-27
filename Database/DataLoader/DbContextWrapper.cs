using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Runtime.CompilerServices;
using LcaDataModel;


namespace LcaDataLoader {
    /// <summary>
    /// DbContextWrapper provides database access convenience methods.
    /// </summary>
    class DbContextWrapper : IDisposable {
        // Data Model DbContext object
        EntityDataModel _DbContext;
        int _CurrentIlcdDataProviderID;

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
        /// Constructor creates model's DbContext
        /// </summary>
        public DbContextWrapper() {
            _DbContext = new EntityDataModel();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCurrentIlcdDataProviderID() {
            return _CurrentIlcdDataProviderID;
        }

        /// <summary>
        /// Create DataProvider record if a record with the same attribute values does not already exist.
        /// </summary>
        /// <param name="dirName">DataProvider.DirName</param>
        /// <param name="name">DataProvider.Name</param>
        /// <returns>DataProvider object created or found.</returns>
        public DataProvider CreateDataProvider(string dirName, string name) {
            DataProvider dataProvider;
            dataProvider = (from dp in _DbContext.DataProviders 
                            where dp.Name.ToLower() == name.ToLower() && dp.DirName.ToLower() == dirName.ToLower() 
                            select dp).FirstOrDefault();
            if (dataProvider == null) {
                dataProvider = new DataProvider { Name = name, DirName = dirName };
                _DbContext.DataProviders.Add(dataProvider);
                SaveChanges();
            }
            else {
                Program.Logger.InfoFormat("Data Provider with Name = {0} and Directory = {1} already exists.", name, dirName);
            }
            _CurrentIlcdDataProviderID = dataProvider.DataProviderID;
            return dataProvider;
        }

        /// <summary>
        /// Invokes DbContext.SaveChanges and handles DbUpdateException.
        /// </summary>
        /// <returns>Number of objects written to the database, or 0 if DbUpdateException occurs.</returns>
        public int SaveChanges() {
            try {
                return _DbContext.SaveChanges();
            }
            catch (DbUpdateException e) {
                Program.Logger.ErrorFormat("Database update exception: {0}", e.Message);
                for (var ie = e.InnerException; ie != null; ie = ie.InnerException) {
                    Program.Logger.ErrorFormat("Inner exception: {0}", ie.InnerException.Message);
                }

                return 0;
            }
        }

        /// <summary>
        /// Insert ILCD entity into the database.
        /// </summary>
        /// <param name="ilcdEntity">An entity with a UUID.</param>
        /// <returns>true iff entity was successfully inserted</returns>
        /// TODO : this method is no longer needed. Replace all references with AddEntity, defined below
        public bool AddIlcdEntity(IIlcdEntity ilcdEntity) {
            bool isAdded = false;

            _DbContext.Set(ilcdEntity.GetType()).Add(ilcdEntity);
            if (SaveChanges() > 0) {
                isAdded = true;
            }
            return isAdded;
        }

        /// <summary>
        /// Insert  entity into the database.
        /// </summary>
        /// <param name="entity">An entity modeled in LcaDataModel</param>
        /// <returns>true iff entity was successfully inserted</returns>
        public bool AddEntity<T>(T entity) where T : class {
            _DbContext.Set<T>().Add(entity);
            return (SaveChanges() > 0);     
        }

        /// <summary>
        /// Insert  list of entities into the database.
        /// </summary>
        /// <param name="entityList">List of entities modeled in LcaDataModel</param>
        public void AddEntities<T>(List<T> entityList) where T : class  {
            _DbContext.Set<T>().AddRange(entityList);
            SaveChanges();
        }

        /// <summary>
        /// Create an entity with a given ID, if the ID does not already exist, and add it to the data model.
        /// Changes are not saved so that other properties can be set before saving.
        /// Use this when loading entities from CSV.
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>New or existing entity with matching ID</returns>
        public T CreateEntityWithID<T>(int id) where T : class, IEntity, new() {
            T ent = Find<T>(id);
            if (ent == null) {
                ent = new T { ID = id };
                _DbContext.Set<T>().Add(ent);
            }
            else {
                Program.Logger.WarnFormat("Found {1} with ID = {0}. Entity will not be added.", id, typeof(T).ToString());
            }
            return ent;
        }

        /// <summary>
        /// Populate Lookup table, if it is empty.
        /// </summary>
        /// <param name="lutSet">database context DbSet for the lookup table</param>
        /// <param name="nameList">list of names to be inserted (order is preserved)</param>
        public static void SeedLUT<T>(DbSet<T> lutSet, List<string> nameList) where T : class, ILookupEntity, new() {
            int id = 1;
            if (lutSet.Count() == 0) {
                foreach (string name in nameList) {
                    lutSet.Add(new T { ID = id++, Name = name });
                }
            }
            else {
                Program.Logger.ErrorFormat("Lookup table {0} is not empty.", typeof(T).ToString());
            }
        }

        /// <summary>
        /// Populate Lookup table from enum, if it is empty.
        /// </summary>
        /// <param name="lutSet">database context DbSet for the lookup table</param>
        /// <param name="enumType">the enum type to use as data source</param>
        public static void SeedLUT<T>(DbSet<T> lutSet, Type enumType) 
            where T : class, ILookupEntity, new() 
            {
            int id = 1;
            if (lutSet.Count() == 0) {
                foreach (string name in Enum.GetNames(enumType)) {
                    lutSet.Add(new T { ID = id++, Name = name });
                }
            }
            else {
                Program.Logger.ErrorFormat("Lookup table {0} is not empty.", typeof(T).ToString());
            }
        }

        /// <summary>
        /// Generic method to look up entity by name.
        /// </summary>
        /// <param name="name">Entity name</param>
        /// <returns>Entity ID, if name was found, otherwise null</returns>
        public int? LookupEntityID<T>(string name) where T : class, ILookupEntity {
            DbSet<T> dbSet = _DbContext.Set<T>();
            ILookupEntity entity = (from le in dbSet where le.Name == name select le).FirstOrDefault();
            if (entity == null) {
                if (typeof(T) == typeof(FlowType)) {
                    return GetFlowTypeID(name);
                }
                Program.Logger.ErrorFormat("Lookup {0} by name, {1}, failed.", typeof(T).ToString(), name);
                return null;
            }
            else {
                return entity.ID;
            }
        }

        /// <summary>
        /// Generic method to retrieve ILCD Entity by UUID.
        /// </summary>
        /// <param name="uuid">UUID value</param>
        /// <returns>Entity, if found, otherwise null</returns>
        public T GetIlcdEntity<T>(string uuid) where T : class, IIlcdEntity {
            DbSet<T> dbSet = _DbContext.Set<T>();
            T entity = (from le in dbSet where le.UUID == uuid select le).FirstOrDefault();
            return entity;
        }

        /// <summary>
        /// Generic method to look up ILCD Entity ID by UUID.
        /// Report error if not found.
        /// </summary>
        /// <param name="uuid">UUID value</param>
        /// <returns>Entity ID, if found, otherwise null</returns>
        public int? GetIlcdEntityID<T>(string uuid) where T : class, IIlcdEntity {
            IIlcdEntity entity = GetIlcdEntity<T>(uuid);
            if (entity == null) {
                Program.Logger.ErrorFormat("Unable to find {0} with UUID, {1}.", typeof(T).ToString(), uuid);
                return null;
            }
            else {
                return entity.ID;
            }
        }

        /// <summary>
        /// Generic method to search for entity by ID
        /// </summary>
        /// <param name="id">ID value</param>
        /// <returns>Entity with ID, if found, otherwise null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Find<T>(int id) where T : class {
            return _DbContext.Set<T>().Find(id);
        }

        /// <summary>
        /// Expose DbSet members for operations on relationship and other unusual tables.
        /// </summary>
        /// <returns>DbSet member variable for type T.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbSet<T> GetDbSet<T>() where T : class {
            return _DbContext.Set<T>();
        }

        /// <summary>
        /// Insert initial data into new database.
        /// </summary>
        /// <param name="dbContext">Entity Framework database context</param>
        public static void Seed(EntityDataModel dbContext) {
            SeedLUT<DataProvider>(dbContext.DataProviders, typeof(DataProviderEnum));
            SeedLUT<DataType>(dbContext.DataTypes, typeof(DataTypeEnum));
            SeedLUT<FlowType>(dbContext.FlowTypes, typeof(FlowTypeEnum)); 
            SeedLUT<ImpactCategory>(dbContext.ImpactCategories,
                new List<string>(new string[] {            
                    "Abiotic resource depletion",
                    "Acidification",
                    "Aquatic eco-toxicity",
                    "Aquatic Eutrophication",
                    "Biotic resource depletion",
                    "Cancer human health effects",
                    "Climate change",
                    "Ionizing radiation",
                    "Land use",
                    "Non-cancer human health effects",
                    "Ozone depletion",
                    "Photochemical ozone creation",
                    "Respiratory inorganics",
                    "Terrestrial Eutrophication",
                    "other"
                    }));
            SeedLUT<IndicatorType>(dbContext.IndicatorTypes,
                new List<string>(new string[] {            
                    "Area of Protection damage indicator",
                    "Combined single-point indicator",
                    "Damage indicator",
                    "Mid-point indicator"
                }));
            SeedLUT<Direction>(dbContext.Directions, typeof(DirectionEnum));
            SeedLUT<ReferenceType>(dbContext.ReferenceTypes,
                new List<string>(new string[] {            
                    "Other parameter",
                    "Reference flow(s)"
             }));
            SeedLUT<NodeType>(dbContext.NodeTypes, typeof(NodeTypeEnum));
            SeedLUT<ParamType>(dbContext.ParamTypes, typeof(ParamTypeEnum));
            SeedLUT<ProcessType>(dbContext.ProcessTypes,
                new List<string>(new string[] {  
                    "Avoided product system",
                    "LCI result",
                    "Partly terminated system",
                    "Unit process, black box",
                    "Unit process, single operation"
             }));
            //SeedLUT<Visibility>(dbContext.Visibilities, typeof(VisibilityEnum));

            if (dbContext.Users.Count() == 0) {
                dbContext.Users.Add(new User { Name = "DB Creator" });
            }

            dbContext.SaveChanges();
        }

        /// <summary>
        /// Use this method to transform ILCD flow type name to database FlowTypeID.
        /// </summary>
        /// <param name="flowTypeName">ILCD flow type as string</param>
        /// <returns>FlowTypeEnum value as int</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetFlowTypeID(string flowTypeName) {
            // Database only has 2 flow types: "ElementaryFlow" and "IntermediateFlow", for all other ILCD flow types. 
            return flowTypeName.Equals("Elementary flow") ? Convert.ToInt32(FlowTypeEnum.ElementaryFlow) : Convert.ToInt32(FlowTypeEnum.IntermediateFlow);
        }

        /// <summary>
        /// Use this method to check if an ILCDEntity with given UUID already exists in the database
        /// </summary>
        /// <param name="uuid">The UUID value</param>
        /// <returns>true iff found</returns>
        public bool IlcdUuidExists(string uuid) {
            ILCDEntity entity = (from il in _DbContext.ILCDEntities where il.UUID == uuid select il).FirstOrDefault();
            return (entity != null);
        }

        /// <summary>
        /// Use this method to check if an entity (any type having internal ID as primary key) already exists in the database
        /// </summary>
        /// <param name="id">The ID value</param>
        /// <returns>true iff found</returns>
        public bool EntityIdExists<T>(int id) where T : class {
            return _DbContext.Set<T>().Find(id) != null ;
        }

        /// <summary>
        /// Use this method to fill in missing LCIA FlowID references whenever Flows have been inserted after LCIA import.
        /// Could not see how to do this in EF/LINQ, so this method uses SQL command. 
        /// </summary>
        /// <returns>Number of records updated</returns>
        public int UpdateLciaFlowID() {
            _DbContext.Database.ExecuteSqlCommand(
                "UPDATE LCIA SET FlowID = (SELECT FlowID FROM Flow WHERE LCIA.FlowUUID = Flow.UUID) " +
                "WHERE  FlowID IS NULL AND EXISTS (SELECT FlowID FROM Flow WHERE LCIA.FlowUUID = Flow.UUID)");
            return _DbContext.SaveChanges();
        }
    }
}
