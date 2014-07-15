using Data;
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
        void Traverse(int scenarioId = 0);
        IEnumerable<DependencyParamModel> ApplyDependencyParam(int scenarioId = 1);
        IEnumerable<FlowPropertyParamModel> ApplyFlowPropertyParam(int scenarioId = 1);
    }
}
