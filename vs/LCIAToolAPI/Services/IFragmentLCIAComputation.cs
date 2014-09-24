using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IFragmentLCIAComputation
    {
        void FragmentLCIACompute(int fragmentId, int scenarioId);
        IEnumerable<FragmentLCIAModel> ComputeFragmentLCIA(int? fragmentId, int? scenarioId, int? lciaMethodId);
    }
}
