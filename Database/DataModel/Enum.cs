
namespace LcaDataModel {
    using System;

    public enum DataProviderEnum {
        append,
        fragments,
        scenarios
    }

    public enum DataTypeEnum {
        Flow,
        FlowProperty,
        Process,
        UnitGroup,
        Source,
        LCIAMethod,
        Contact,
        Fragment
    }

    public enum DirectionEnum {
        Input, Output
    }

    public enum FlowTypeEnum {
         IntermediateFlow,
         ElementaryFlow
    }

    public enum NodeTypeEnum {
        Process, Fragment, InputOutput, Background
    }
}
