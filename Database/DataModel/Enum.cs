
namespace LcaDataModel {
    using System;

    public enum DataSourceEnum {
        append=1,
        fragments,
        scenarios
    }

    public enum DataTypeEnum {
        Flow=1,
        FlowProperty=2,
        Process=3,
        UnitGroup=4,
        Source=5,
        LCIAMethod=6,
        Contact=7,
        Fragment=8
    }

    public enum DataPathEnum
    {
        flows=1,
        flowproperties=2,
        processes=3,
        unitgroups=4,
        sources=5,
        lciamethods=6,
        contacts=7,
        fragments=8
    };


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
