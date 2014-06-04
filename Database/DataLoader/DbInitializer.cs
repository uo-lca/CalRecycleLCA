using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;

namespace LcaDataLoader {
    /// <summary>
    /// Database initializer - currently set to drop and recreate database.
    /// This will fail if database is in use.
    /// Uses DbContextWrapper to seed database.
    /// </summary>
    public class DbInitializer : DropCreateDatabaseAlways<EntityDataModel> {
        protected override void Seed(EntityDataModel context) {
            DbContextWrapper.Seed(context);
        }
    }
}
