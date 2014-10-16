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
        int _CurrentIlcdDataSourceID;

        // Punctuation characters used to determine where to truncate name
        static char[] _NameDelimiters = new char[] { '(', '.', ',', ':', ';' };

        // Map UUID to specific entity ID (FlowID, FlowPropertyID, etc.) for fast lookup
        Dictionary<string, int> _EntityIdDictionary = new Dictionary<string, int>();

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
        /// Shorten a name
        /// </summary>
        /// <param name="name">The name to shorten</param>
        /// <param name="length">Maximum length of short name</param>
        /// <returns>The short name</returns>
        public string ShortenName(string name, int length) {
            // If text in shorter or equal to length, just return it
            if (name.Length <= length) {
                return name;
            }

            // name is longer, so try to find out where to cut
            int index = name.LastIndexOfAny(_NameDelimiters, length);
            if (index < length * 2 / 3) {
                index = name.LastIndexOf(' ', length);
            }
            if (index == -1) {
                index = length;
            }
            return name.Substring(0, index);
        }

        /// <summary>
        /// Constructor creates model's DbContext
        /// </summary>
        public DbContextWrapper() {
            _DbContext = new EntityDataModel("name=EntityDataModel");
            _DbContext.SyncObjectStateEnabled = false;
            _DbContext.Database.Initialize(false);
        }

        /// <summary>
        /// Enable/disable automatic detection of changes by Entity Framework.
        /// Disable to improve performance of Adds. 
        /// Enable whenever objects are updated.
        /// </summary>
        /// <param name="enabled">set to true to enable, false to disable</param>
        public void SetAutoDetectChanges(bool enabled) {
            _DbContext.Configuration.AutoDetectChangesEnabled = enabled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCurrentIlcdDataSourceID() {
            return _CurrentIlcdDataSourceID;
        }

        /// <summary>
        /// Create DataSource record if a record with the same attribute values does not already exist.
        /// </summary>
        /// <param name="dirName">DataSource.DirName</param>
        /// <param name="name">DataSource.Name</param>
        /// <returns>DataSource object created or found.</returns>
        public DataSource CreateDataSource(string dirName, string name, bool isPrivate) {
            DataSource dataSource;
            int visID = isPrivate ? Convert.ToInt32(VisibilityEnum.Private) : Convert.ToInt32(VisibilityEnum.Public);
            dataSource = (from dp in _DbContext.DataSources 
                            where dp.Name.ToLower() == name.ToLower() && dp.DirName.ToLower() == dirName.ToLower() 
                            select dp).FirstOrDefault();
            if (dataSource == null) {
                dataSource = new DataSource { Name = name, DirName = dirName, VisibilityID = visID };
                _DbContext.DataSources.Add(dataSource);
                SaveChanges();
            }
            else {
                Program.Logger.InfoFormat("Data Source with Name = {0} and Directory = {1} already exists.", name, dirName);
            }
            _CurrentIlcdDataSourceID = dataSource.DataSourceID;
            return dataSource;
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
                    Program.Logger.ErrorFormat("Inner exception: {0}", ie.Message);
                }

                return 0;
            }
        }

        /// <summary>
        /// Insert ILCD entity into the database and cache ID for future lookup.
        /// </summary>
        /// <param name="ilcdEntity">An entity with a UUID.</param>
        /// <param name="uuid">the UUID</param>
        /// <param name="version">only provide version when external key is UUID + Version</param>
        /// <returns>true iff entity was successfully inserted</returns>
        public bool AddIlcdEntity(IIlcdEntity ilcdEntity, string uuid, string version=null) {
            bool isAdded = false;
            _DbContext.Set(ilcdEntity.GetType()).Add(ilcdEntity);
            if (SaveChanges() > 0) {
                _EntityIdDictionary[makeKey(uuid, version)] = ilcdEntity.ID;
                isAdded = true;
            }
            return isAdded;
        }

        /// <summary>
        /// Search for entity by UUID and output its specific entity ID.
        /// Use this method to get the entity ID of an ILCD entity that should 
        /// have been imported into the database.
        /// For example, if T is Flow, and a flow with the given UUID was previously loaded,
        /// return its FlowID.
        /// Searches local cache first, then database. 
        /// If found in database, updates cache.
        /// If not found, logs an error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ilcdDb">Current DbContextWrapper instance</param>
        /// <param name="uuid">The UUID</param>
        /// <param name="outID">Output: Entity ID in its own table</param>
        /// <param name="version">Only provide version when external key is UUID + Version</param>
        /// <returns>true iff the entity was found</returns>
        public bool FindRefIlcdEntityID<T>(string uuid, out int outID, string version=null) where T : class, IIlcdEntity {
            string externalKey = makeKey(uuid, version);
            bool found = _EntityIdDictionary.TryGetValue(externalKey, out outID);
            if (!found) {
                IIlcdEntity entity = GetIlcdEntity<T>(uuid, version);
                if (entity == null) {
                    Program.Logger.ErrorFormat("Unable to find {0} with key, {1}.", typeof(T).ToString(), externalKey);
                    found = false;
                }
                else  {
                    outID = entity.ID;
                    _EntityIdDictionary[externalKey] = outID;
                    found = true;
                }
            }
            return found;
        }

        /// <summary>
        /// Insert entity into the database.
        /// Use to add non-ILCD entities (have no UUID)
        /// </summary>
        /// <param name="entity">An entity modeled in LcaDataModel</param>
        /// <returns>true iff entity was successfully inserted</returns>
        public bool AddEntity<T>(T entity) where T : class {
            _DbContext.Set<T>().Add(entity);
            return (SaveChanges() > 0);     
        }

        /// <summary>
        /// Insert  list of entities into the database.
        /// Use to import relationships from ILCD data.
        /// </summary>
        /// <param name="entityList">List of entities modeled in LcaDataModel</param>
        public void AddEntities<T>(List<T> entityList) where T : class  {
            _DbContext.Set<T>().AddRange(entityList);
            SaveChanges();
        }
        
        /// <summary>
        /// Find entity with a given ID and create it if it does not already exist.
        /// Changes are not saved so that other properties can be set before saving.
        /// New entity must be Added after updating properties in case AutoDetectChanges enabled.
        /// Use this when loading non-ILCD entities from CSV. 
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">Entity ID</param>
        /// <param name="isNew">Output: flags when a new entity was created.</param>
        /// <returns>New or existing entity with matching ID</returns>
        public T ProduceEntityWithID<T>(int id, out bool isNew) where T : class, IEntity, new() {
            T ent = Find<T>(id);
            if (ent == null) {
                ent = new T { ID = id };
                isNew = true;
            }
            else {
                Program.Logger.WarnFormat("Found {1} with ID = {0}. Entity will not be added, but may be updated.", id, typeof(T).ToString());
                isNew = false;
            }
            return ent;
        }

        /// <summary>
        /// Create an entity with a given ID, if the ID does not already exist, and add it to the data model.
        /// Changes are not saved so that other properties can be set before saving.
        /// Use this when loading entities from CSV.
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>New or existing entity with matching ID</returns>
        /// TODO: Replace usage of this method with new method above
        public T CreateEntityWithID<T>(int id) where T : class, IEntity, new() {
            T ent = Find<T>(id);
            if (ent == null) {
                ent = new T { ID = id };
                _DbContext.Set<T>().Add(ent);
            }
            else {
                Program.Logger.WarnFormat("Found {1} with ID = {0}. Entity will not be added, but may be updated.", id, typeof(T).ToString());
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
        /// Generic method to lookup ILCD Entity by UUID and (optionally) Version. 
        /// Return the object from its specific class (e.g., Flow, Process).
        /// </summary>
        /// <param name="uuid">UUID value</param>
        /// <param name="version">Only needed for ILCD data types where multiple versions may be imported (Process)</param>
        /// <returns>IIlcdEntity object, if found, otherwise null</returns>
        public T GetIlcdEntity<T>(string uuid, string version = null) where T : class, IIlcdEntity {
            DbSet<T> dbSet = _DbContext.Set<T>();
            T entity = String.IsNullOrEmpty(version) ?
                (from le in dbSet where le.ILCDEntity.UUID == uuid select le).FirstOrDefault()
                :
                (from le in dbSet
                 where le.ILCDEntity.UUID == uuid && le.ILCDEntity.Version == version
                 select le).FirstOrDefault()
                 ;

            return entity;
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
            SeedLUT<DataSource>(dbContext.DataSources, typeof(DataSourceEnum));
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
            SeedLUT<Visibility>(dbContext.Visibilities, typeof(VisibilityEnum));

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
        /// Use this method to search ILCDEntity by UUID 
        /// and optionally, Version
        /// </summary>
        /// <param name="uuid">The UUID value</param>
        /// <param name="version">Only needed for ILCD data type where multiple versions may be imported (Process)</param>
        /// <returns>Instance of ILCDEntity if found, otherwise null</returns>
        public ILCDEntity GetIlcdEntity(string uuid, string version=null) {
            ILCDEntity entity =
                String.IsNullOrEmpty(version) ?
                (from il in _DbContext.ILCDEntities where il.UUID == uuid select il).FirstOrDefault()
                :
                (from il in _DbContext.ILCDEntities where il.UUID == uuid && il.Version == version select il).FirstOrDefault();
            return entity;
        }

        private string makeKey(string uuid, string version=null) {
            return String.IsNullOrEmpty(version) ? uuid : String.Format("{0} {1}", uuid, version);
        }

        /// <summary>
        /// Use this method before creating an ILCD entity. 
        /// Checks if an ILCD entity with the same UUID and (optionally) Version already exists in the database.
        /// In this case, do not create.
        /// Version should only be provided for data type where UUID and Version is the external key (Process). 
        /// If an entity is found, this method logs a warning, and caches the ID for future lookup.
        /// </summary>
        ///  /// <param name="uuid">The UUID value</param>
        /// <returns>true iff found</returns>
        public bool IlcdEntityAlreadyExists<T>(string uuid, string version=null) where T : class, IIlcdEntity {
            IIlcdEntity entity = GetIlcdEntity<T>(uuid, version);
            if (entity == null) {
                return false;
            }
            else {
                string externalKey = makeKey(uuid, version);
                Program.Logger.WarnFormat("{0} with key {1} was already imported.", typeof(T).ToString(), externalKey);
                _EntityIdDictionary[externalKey] = Convert.ToInt32(entity.ID);
                return true;
            }
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
            string flowIDQuery = "(SELECT FlowID FROM Flow INNER JOIN ILCDEntity ON Flow.ILCDEntityID = ILCDEntity.ILCDEntityID WHERE LCIA.FlowUUID = ILCDEntity.UUID)";
            string updateCmd = string.Format("UPDATE LCIA SET FlowID = {0} WHERE  FlowID IS NULL AND EXISTS {0}", flowIDQuery);
            _DbContext.Database.ExecuteSqlCommand(updateCmd);
            return _DbContext.SaveChanges();
        }


    }
}
