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

        // Dictionary maps UUID to Entity ID
        Dictionary<string, int> _UuidDictionary = new Dictionary<string, int>();

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
        /// Constructor injects model's DbContext
        /// </summary>
        public DbContextWrapper(EntityDataModel dbContext) {
            _DbContext = dbContext;
        }

        /// <summary>
        /// Invokes DbContext.SaveChanges and handles DbUpdateException.
        /// </summary>
        public int SaveChanges() {
            try {
                return _DbContext.SaveChanges();
            }
            catch (DbUpdateException e) {
                Console.WriteLine("ERROR: Database update exception!");
                Console.WriteLine(e.Message);
                for (var ie = e.InnerException; ie != null; ie = ie.InnerException) {
                    Console.WriteLine("Inner Exception: {0}", e.InnerException.Message);
                }

                return 0;
            }
        }

        /// <summary>
        /// Insert ILCD entity into the database.
        /// </summary>
        /// <param name="ilcdEntity">An entity with a UUID.</param>
        /// <returns>true iff entity was successfully inserted</returns>
        public bool AddIlcdEntity(IIlcdEntity ilcdEntity) {
            bool isAdded = false;

            _DbContext.Set(ilcdEntity.GetType()).Add(ilcdEntity);
            if (SaveChanges() > 0) {
                _UuidDictionary.Add(ilcdEntity.UUID, ilcdEntity.ID);
                isAdded = true;
            }
            return isAdded;
        }

        public void AddUnitConversions(List<UnitConversion> unitConversionList) {
            foreach (var unitConversion in unitConversionList) {
                _DbContext.UnitConversions.Add(unitConversion);
            }
            _DbContext.SaveChanges();
        }

        public void AddFlowFlowProperties(List<FlowFlowProperty> flowFlowPropertyList) {
            foreach (var flowFlowProperty in flowFlowPropertyList) {
                _DbContext.FlowFlowProperties.Add(flowFlowProperty);
            }
            _DbContext.SaveChanges();
        }

        public void AddEntities<T>(List<T> entityList) where T : class, IEntity  {
            _DbContext.Set<T>().AddRange(entityList);
            _DbContext.SaveChanges();
        }

        public static void SeedLUT<T>(DbSet<T> lutSet, List<string> nameList) where T : class, ILookupEntity, new() {
            int id = 1;
            if (lutSet.Count() == 0) {
                foreach (string name in nameList) {
                    lutSet.Add(new T { ID = id++, Name = name });
                }
            }
            else {
                Console.WriteLine("ERROR: Lookup table {0} is not empty.", typeof(T).ToString());
            }
        }

        public int? LookupEntityID<T>(string name) where T : class, ILookupEntity {
            DbSet<T> dbSet = _DbContext.Set<T>();
            ILookupEntity entity = (from le in dbSet where le.Name == name select le).FirstOrDefault();
            if (entity == null) {
                if (typeof(T) == typeof(FlowType)) {
                    return GetFlowTypeID(name);
                }
                Console.WriteLine("ERROR: Lookup {0} by name, {1}, failed.", typeof(T).ToString(), name);
                return null;
            }
            else {
                return entity.ID;
            }
        }

        public int? GetIlcdEntityID<T>(string uuid) where T : class, IIlcdEntity {
            DbSet<T> dbSet = _DbContext.Set<T>();
            IIlcdEntity entity = (from le in dbSet where le.UUID == uuid select le).FirstOrDefault();
            if (entity == null) {
                Console.WriteLine("ERROR: Unable to find {0} with UUID, {1}.", typeof(T).ToString(), uuid);
                return null;
            }
            else {
                return entity.ID;
            }
        }

        public static void Seed(EntityDataModel dbContext) {
            SeedLUT<DataProvider>(dbContext.DataProviders,
                new List<string>(new string[] {            
                    "append",
                    "fragments",
                    "scenarios"
             }));
            SeedLUT<FlowType>(dbContext.FlowTypes,
                new List<string>(new string[] {            
                    "Intermediate Flow",
                    "Elementary Flow"
             })); 
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
            SeedLUT<Direction>(dbContext.Directions,
                new List<string>(new string[] {            
                    "Input",
                    "Output"
             }));
            dbContext.SaveChanges();

        }

        /// <summary>
        /// Use this method to transform ILCD flow type name to database FlowTypeID.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetFlowTypeID(string flowTypeName) {
            // Database only has 2 flow types: "Elementary Flow" and "Intermediate Flow", for all other ILCD flow types. 
            return flowTypeName.Equals("Elementary flow") ? 2 : 1;
        }
        
        public bool IlcdUuidExists(string uuid) {
            ILCDEntity entity = (from il in _DbContext.ILCDEntities where il.UUID == uuid select il).FirstOrDefault();
            return (entity != null);
        }

    }
}
