using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;

namespace LcaDataLoader {
    public class DbInitializer : DropCreateDatabaseAlways<EntityDataModel> {
        protected override void Seed(EntityDataModel context) {
            DbContextWrapper.Seed(context);
        }
    }
}
