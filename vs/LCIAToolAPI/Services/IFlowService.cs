using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IFlowService : IService<Flow>
    {
        IEnumerable<Flow> GetFlowsInFragment(int fragmentId);            
    }
}
