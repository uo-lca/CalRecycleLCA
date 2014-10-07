using LcaDataModel;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IFlowPropertyParamService : IService<FlowPropertyParam>
    {
        IEnumerable<FlowPropertyParam> GetFlowPropertyParams(int scenarioId = 1);
    }
}
