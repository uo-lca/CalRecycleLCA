using LcaDataModel;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IFlowPropertyService : IService<FlowProperty>
    {
        IEnumerable<FlowPropertyModel> GetFlowPropertiesByFragment(int fragmentId);            
    }
}
