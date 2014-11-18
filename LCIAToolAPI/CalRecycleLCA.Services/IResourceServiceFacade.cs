using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public interface IResourceServiceFacade {
        IEnumerable<LCIAMethodResource> GetActiveLCIAMethodResources(int? impactCategoryID = null);
        IEnumerable<ScenarioResource> GetScenarios();
        IEnumerable<ScenarioResource> GetScenarios(int ScenarioGroupID);
        IEnumerable<FragmentResource> GetFragmentResources();
        FragmentResource GetFragmentResource(int fragmentID);
        IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = 0);
        IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID);
        IEnumerable<ProcessFlowResource> GetProcessFlows(int processID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID);
        LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID = 0);
        IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID, int scenarioGroupID = 1);
        IEnumerable<ProcessResource> GetProcesses(int? flowTypeID = null);
        IEnumerable<ImpactCategoryResource> GetImpactCategories();
        LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID, int scenarioID = 0);
        IEnumerable<FlowTypeResource> GetFlowTypes();
    }
}
