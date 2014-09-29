using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Repository;

namespace LcaDataModel {
    /// <summary>
    /// Interface to common Entity properties. Hides differences in column names.
    /// All Entities have ID.
    /// </summary>
    public interface IEntity {
        // ID accessor signatures, do not persist
        [NotMapped]
        int ID { get; set; }
    }

    public partial class Background : Entity, IEntity
    {
        [NotMapped]
        public int ID {
            get { return BackgroundID; }
            set { BackgroundID = value; }
        }
    }

    public partial class Category : Entity, IEntity{
        [NotMapped]
        public int ID {
            get { return CategoryID; }
            set { CategoryID = value; }
        }
    }

    public partial class CategorySystem : Entity, IEntity
    {
        [NotMapped]
        public int ID {
            get { return CategorySystemID; }
            set { CategorySystemID = value; }
        }
    }

    public partial class CompositionModel : IEntity {
        [NotMapped]
        public int ID {
            get { return CompositionModelID; }
            set { CompositionModelID = value; }
        }
    }

    public partial class CompositionData : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return CompositionDataID; }
            set { CompositionDataID = value; }
        }
    }

    public partial class DependencyParam : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return DependencyParamID; }
            set { DependencyParamID = value; }
        }
    }

    // Weak entity - related to DependencyParam
    public partial class DistributionParam : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return DependencyParamID; }
            set { DependencyParamID = value; }
        }
    }

    public partial class FlowFlowProperty : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return FlowFlowPropertyID; }
            set { FlowFlowPropertyID = value; }
        }
    }

    public partial class FragmentFlow : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return FragmentFlowID; }
            set { FragmentFlowID = value; }
        }
    }

    public partial class FragmentNodeProcess : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return FragmentNodeProcessID; }
            set { FragmentNodeProcessID = value; }
        }
    }

    public partial class FragmentNodeFragment : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return FragmentNodeFragmentID; }
            set { FragmentNodeFragmentID = value; }
        }
    }

    public partial class LCIA : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return LCIAID; }
            set { LCIAID = value; }
        }
    }

    public partial class Param : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return ParamID; }
            set { ParamID = value; }
        }
    }

    public partial class ProcessComposition : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return ProcessCompositionID; }
            set { ProcessCompositionID = value; }
        }
    }

    public partial class ProcessFlow : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return ProcessFlowID; }
            set { ProcessFlowID = value; }
        }
    }

    public partial class Scenario : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return ScenarioID; }
            set { ScenarioID = value; }
        }
    }

    public partial class ScenarioGroup : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return ScenarioGroupID; }
            set { ScenarioGroupID = value; }
        }
    }

    public partial class UnitConversion : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return UnitConversionID; }
            set { UnitConversionID = value; }
        }
    }

    public partial class User : Entity, IEntity {
        [NotMapped]
        public int ID {
            get { return UserID; }
            set { UserID = value; }
        }
    }

    /// <summary>
    /// Interface to common ILCD Entity properties. Hides differences in column names.
    /// All ILCD entities have have a reference to ILCDEntity, the table where UUIDs 
    /// and metadata are stored.
    /// </summary>
    public interface IIlcdEntity : IEntity {
        // UUID accessor signature  
        [NotMapped]
        //string UUID { get; set; }
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

    public partial class Process : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return ProcessID; }
            set { ProcessID = value; }
        }
    }

  

    public partial class Fragment : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return FragmentID; }
            set { FragmentID = value; }
        }
    }

    public partial class Classification : IIlcdEntity {
        [NotMapped]
        public int ID {
            get { return ClassificationID; }
            set { ClassificationID = value; }
        }
    }

    /// <summary>
    /// Interface to common Lookup Table properties. Hides differences in ID column name.
    /// All Lookup entities have Name
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

    public partial class DataType : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return DataTypeID; }
            set { DataTypeID = ID; }
        }
    }

    public partial class Direction : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return DirectionID; }
            set { DirectionID = ID; }
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

    public partial class NodeType : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return NodeTypeID; }
            set { NodeTypeID = ID; }
        }
    }

    public partial class ParamType : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return ParamTypeID; }
            set { ParamTypeID = ID; }
        }
    }

    public partial class ProcessType : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return ProcessTypeID; }
            set { ProcessTypeID = ID; }
        }
    }

    public partial class ReferenceType : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return ReferenceTypeID; }
            set { ReferenceTypeID = ID; }
        }
    }

    public partial class Visibility : ILookupEntity {
        [NotMapped]
        public int ID {
            get { return VisibilityID; }
            set { VisibilityID = ID; }
        }
    }
}


