using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LcaDataModel {
    /// <summary>
    /// Interface to common Entity properties. Hides differences in column names.
    /// </summary>
    public interface IEntity {
        // ID accessor signatures, do not persist
        [NotMapped]
        int ID { get; set; }
    }

    /// <summary>
    /// Interface to common ILCD Entity properties. Hides differences in column names.
    /// </summary>
    public interface IIlcdEntity : IEntity {
        // UUID accessor signature  
        [NotMapped]
        string UUID { get; set; }
    }

    public partial class UnitGroup : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return UnitGroupID; }
            set { UnitGroupID = value; }
        }

        public string UUID {
            get { return UnitGroupUUID; }
            set { UnitGroupUUID = value; }
        }
    }

    public partial class FlowProperty : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return FlowPropertyID; }
            set { FlowPropertyID = value; }
        }

        public string UUID {
            get { return FlowPropertyUUID; }
            set { FlowPropertyUUID = value; }
        }
    }

    public partial class Flow : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return FlowID; }
            set { FlowID = value; }
        }

        public string UUID {
            get { return FlowUUID; }
            set { FlowUUID = value; }
        }
    }
}
