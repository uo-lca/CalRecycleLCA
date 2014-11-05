using LcaDataModel;
using Entities.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface ITestGenericService
    {
        IEnumerable<LCIAMethod> GetLCIAMethods();
        FragmentNodeResource FindTerminus(int fragmentFlowID, int scenarioID);
        IEnumerable<InventoryModel> GetDependencies(int fragmentFlowId, int scenarioId);
    }
}
