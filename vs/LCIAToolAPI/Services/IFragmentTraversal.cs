using LcaDataModel;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IFragmentTraversal
    {
        void Traverse(int fragmentId = 11, int scenarioId = 1);
        IEnumerable<DependencyParamModel> ApplyDependencyParam(int fragmentId, int scenarioId = 1);
        IEnumerable<FlowPropertyParamModel> ApplyFlowPropertyParam(int fragmentId, int scenarioId = 1);
    }
}
