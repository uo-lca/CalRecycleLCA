
namespace LcaDataModel {
    using System;

    public enum DataProviderEnum {
        append=1,
        fragments,
        scenarios
    }

    public enum DataTypeEnum {
        Flow=1,
        FlowProperty,
        Process,
        UnitGroup,
        Source,
        LCIAMethod,
        Contact,
        Fragment
    }

    public enum DirectionEnum {
        Input=1, Output
    }

    public enum FlowTypeEnum {
         IntermediateFlow=1,
         ElementaryFlow
    }

    public enum NodeTypeEnum {
        Process=1, Fragment, InputOutput, Background, Cutoff
    }

    public enum ParamTypeEnum {
        Dependency = 1, 
        Conservation,
        Distribution,
        FlowProperty,
        UNUSED, // Formerly, CompositionProperty
        ProcessDissipation,
        NodeDissipation, 
        ProcessEmission,
        NodeEmission, 
        LCIAFactor
    }

    public enum VisibilityEnum {
        Public = 1, Private
    }
}
