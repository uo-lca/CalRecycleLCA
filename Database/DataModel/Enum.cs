
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

    public enum NodeTypeEnum {
        Process, Fragment, InputOutput, Background
    }
}
