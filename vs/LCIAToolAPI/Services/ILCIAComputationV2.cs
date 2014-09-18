using Entities.Models;
using LcaDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ILCIAComputationV2
    {
        IEnumerable<InventoryModel> ComputeProcessLCI(int? processId, int? scenarioId = 1);
        IEnumerable<ProcessFlow> ComputeProcessDissipation(int processId, int scenarioId = 1);
        IEnumerable<LCIAModel> ComputeProcessLCIA(IEnumerable<InventoryModel> inventory, LCIAMethod lciaMethodItem, int? scenarioId = 1);
        double LCIACompute(int processId, int scenarioId);
        double ProcessLCIA(int? processId, IEnumerable<LCIAMethod> lciaMethods, int? scenarioId = 1);
    }
}
