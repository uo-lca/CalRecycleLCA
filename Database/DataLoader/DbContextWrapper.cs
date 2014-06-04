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
