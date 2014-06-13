using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;

namespace LcaDataLoader {
    /// <summary>
    /// Database initializer - drops and recreates database.
    /// This will fail if database is in use.
    /// Uses DbContextWrapper to seed database.
    /// </summary>
    public class DropCreateDatabaseInitializer : DropCreateDatabaseAlways<EntityDataModel> {
        protected override void Seed(EntityDataModel context) {
            DbContextWrapper.Seed(context);
        }
    }
}
