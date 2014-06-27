
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
        Process=1, Fragment, InputOutput, Background
    }
}
