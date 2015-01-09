using Entities.Models;
using LcaDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface ILCIAComputationV2
    {
        IEnumerable<InventoryModel> ComputeProcessLCI(int? processId, int? scenarioId);
        IEnumerable<ProcessFlow> ComputeProcessDissipation(int processId, int scenarioId);
        //IEnumerable<LCIAModel> ComputeProcessLCIA(IEnumerable<InventoryModel> inventory, LCIAMethod lciaMethodItem, int? scenarioId);
        IEnumerable<LCIAResult> LCIACompute(int processId, int scenarioId);
        IEnumerable<LCIAResult> ProcessLCIA(int? processId, IEnumerable<int> lciaMethods, int? scenarioId);
    }
}
