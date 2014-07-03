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
        IEnumerable<DependencyParamModel> ApplyDependencyParam(int scenarioId = 0);
        IEnumerable<FlowPropertyParamModel> ApplyFlowPropertyParam(int scenarioId = 0);
    }
}
