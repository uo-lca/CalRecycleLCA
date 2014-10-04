using LcaDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;

namespace CalRecycleLCA.Services
{
    public interface IFlowPropertyParamService : IService<FlowPropertyParam>
    {
        IEnumerable<FlowPropertyParam> GetFlowPropertyParams(int scenarioId = 1);
    }
}
