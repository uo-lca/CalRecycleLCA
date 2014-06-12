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
        public DbContextWrapper(EntityDataModel dbContext)
        {
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
                for (var ie = e.InnerException; ie != null; ie = ie.InnerException ) {
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
            if (_UuidDictionary.ContainsKey(ilcdEntity.UUID)) {
                Console.WriteLine("WARNING: Unable to add entity because UUID {0} already exists.", ilcdEntity.UUID);
            }
            else {
                _DbContext.Set(ilcdEntity.GetType()).Add(ilcdEntity);
                if (SaveChanges() > 0) {
                    _UuidDictionary.Add(ilcdEntity.UUID, ilcdEntity.ID);
                    isAdded = true;
                }
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

        public static void Seed(EntityDataModel dbContext) {
            if (dbContext.DataProviders.Count() == 0) {
                dbContext.DataProviders.Add(
                    new DataProvider { Name = "append" }
                );
                dbContext.DataProviders.Add(
                    new DataProvider { Name = "fragments" }
                );
                dbContext.DataProviders.Add(
                    new DataProvider { Name = "scenarios" }
                );
                dbContext.SaveChanges();
            }
            else {
                Console.WriteLine("WARNING: DataProvider table is not empty and will not be seeded.");
            }
            if (dbContext.FlowTypes.Count() == 0) {
                dbContext.FlowTypes.Add(
                    new FlowType { Name = "Intermediate Flow" }
                );
                dbContext.FlowTypes.Add(
                    new FlowType { Name = "Elementary Flow" }
                );
                dbContext.SaveChanges();
            }
            else {
                Console.WriteLine("WARNING: FlowType table is not empty and will not be seeded.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetFlowTypeID(string flowTypeName) {
            return flowTypeName.Equals("Elementary flow") ? 2 : 1;
        }

        public int? GetID(string uuid) {
            if ( _UuidDictionary.ContainsKey(uuid)) {
                return _UuidDictionary[uuid];
            }
            else {
                return null;
            }
        }
        
    }
}
