using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using LcaDataModel;


namespace LcaDataLoader {
    class DbContextWrapper : IDisposable {

        EntityDataModel _DbContext;
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


        public DbContextWrapper(EntityDataModel dbContext)
        {
            _DbContext = dbContext;
        }

        //
        // Summary:
        //     Saves all changes made in this context to the underlying database.
        //
        // Returns:
        //     The number of objects written to the underlying database.
        //
        // Unhandled Exceptions:
        //
        //   System.Data.Entity.Infrastructure.DbUpdateConcurrencyException:
        //     A database command did not affect the expected number of rows. This usually
        //     indicates an optimistic concurrency violation; that is, a row has been changed
        //     in the database since it was queried.
        //
        //   System.Data.Entity.Validation.DbEntityValidationException:
        //     The save was aborted because validation of entity property values failed.
        //
        //   System.NotSupportedException:
        //     An attempt was made to use unsupported behavior such as executing multiple
        //     asynchronous commands concurrently on the same context instance.
        //
        //   System.ObjectDisposedException:
        //     The context or connection have been disposed.
        //
        //   System.InvalidOperationException:
        //     Some error occurred attempting to process entities in the context either
        //     before or after sending commands to the database.
        //
        public int SaveChanges() {
            try {
                return _DbContext.SaveChanges();
            }
            catch (DbUpdateException e) {
                Console.WriteLine("ERROR: Database update exception!");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException.Message);
                return 0;
            }
        }

        public bool AddUnitGroup(UnitGroup unitGroup) {
            bool isAdded = false;
            if ( _UuidDictionary.ContainsKey(unitGroup.UnitGroupUUID)) {
                Console.WriteLine("WARNING: Unable to add unit group because UUID {0} already exists.", unitGroup.UnitGroupUUID);
            }
            else {
                _DbContext.UnitGroups.Add(unitGroup);
                if (SaveChanges() > 0) {
                    _UuidDictionary.Add(unitGroup.UnitGroupUUID, unitGroup.UnitGroupID);
                    isAdded = true;
                }
            }
            return isAdded;
        }

        public bool AddFlowProperty(FlowProperty flowProperty) {
            bool isAdded = false;
            if (_UuidDictionary.ContainsKey(flowProperty.FlowPropertyUUID)) {
                Console.WriteLine("WARNING: Unable to add flow property because UUID {0} already exists.", flowProperty.FlowPropertyUUID);
            }
            else {
                _DbContext.FlowProperties.Add(flowProperty);
                if (SaveChanges() > 0) {
                    _UuidDictionary.Add(flowProperty.FlowPropertyUUID, flowProperty.FlowPropertyID);
                    isAdded = true;
                }
            }
            return isAdded;
        }

        public bool AddFlow(Flow flow) {
            bool isAdded = false;
            if (_UuidDictionary.ContainsKey(flow.FlowUUID)) {
                Console.WriteLine("WARNING: Unable to add flow because UUID {0} already exists.", flow.FlowUUID);
            }
            else {
                _DbContext.Flows.Add(flow);
                if (SaveChanges() > 0) {
                    _UuidDictionary.Add(flow.FlowUUID, flow.FlowID);
                    isAdded = true;
                }
            }
            return isAdded;
        }

        public void AddUnitConversions(List<UnitConversion> unitConversionList) {
            foreach (var unitConversion in unitConversionList) {
                _DbContext.UnitConversions.Add(unitConversion);
            }
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
                    new FlowType { Name = "Intemediate Flow" }
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

        public int? GetFlowTypeID(string flowTypeName) {
            foreach (var flowType in _DbContext.FlowTypes) {
                if (String.Equals(flowType.Name, flowTypeName, StringComparison.OrdinalIgnoreCase)) {
                    return flowType.FlowTypeID;
                }
            }
            return null;
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
