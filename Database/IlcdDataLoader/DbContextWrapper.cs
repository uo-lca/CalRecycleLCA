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
        Dictionary<string, int> _UnitGroupDictionary = new Dictionary<string, int>();

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
                Console.WriteLine("Database update exception:");
                Console.WriteLine(e.ToString());
                return 0;
            }
        }

        public void AddUnitGroup(UnitGroup unitGroup) {
            _DbContext.UnitGroups.Add(unitGroup);
            _UnitGroupDictionary.Add(unitGroup.UnitGroupUUID, unitGroup.UnitGroupID);
        }

        public void AddUnitConversions(List<UnitConversion> unitConversionList) {
            foreach (var unitConversion in unitConversionList) {
                _DbContext.UnitConversions.Add(unitConversion);
            }
        }

        
    }
}
