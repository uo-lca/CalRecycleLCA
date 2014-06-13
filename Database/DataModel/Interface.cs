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
        ILCDEntity ILCDEntity { get; set; }
    }

    public partial class UnitGroup : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return UnitGroupID; }
            set { UnitGroupID = value; }
        }
    }

    public partial class FlowProperty : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return FlowPropertyID; }
            set { FlowPropertyID = value; }
        }
    }

    public partial class Flow : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return FlowID; }
            set { FlowID = value; }
        }
    }

    public partial class LCIAMethod : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return LCIAMethodID; }
            set { LCIAMethodID = value; }
        }
    }

    /// <summary>
    /// Interface to common Lookup Table properties. Hides differences in ID column name.
    /// </summary>
    public interface ILookupEntity : IEntity {
        string Name { get; set; }
    }

    public partial class DataProvider : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return DataProviderID; }
            set { DataProviderID = ID; }
        }
    }

    public partial class FlowType : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return FlowTypeID; }
            set { FlowTypeID = ID; }
        }
    }

    public partial class ImpactCategory : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return ImpactCategoryID; }
            set { ImpactCategoryID = ID; }
        }
    }

    public partial class IndicatorType : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return IndicatorTypeID; }
            set { IndicatorTypeID = ID; }
        }
    }
}


