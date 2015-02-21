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
        // these are public only for diagnostic purposes
        IEnumerable<InventoryModel> ComputeProcessEmissions(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<InventoryModel> ComputeProcessDissipation(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        // this is the main inventory result
        List<ProcessFlowResource> ComputeProcessLCI(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        //IEnumerable<LCIAModel> ComputeProcessLCIA(IEnumerable<InventoryModel> inventory, LCIAMethod lciaMethodItem, int? scenarioId);
        IEnumerable<LCIAResult> LCIACompute(int processId, int scenarioId); // wrapper for ProcessLCIA to include all LCIA methods
        IEnumerable<LCIAResult> ProcessLCIA(int processId, IEnumerable<int> lciaMethods, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
    }
}
