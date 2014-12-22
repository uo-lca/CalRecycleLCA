using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using LcaDataModel;

namespace CalRecycleLCA.Services
{
    public interface IResourceServiceFacade {
        IEnumerable<LCIAMethodResource> GetActiveLCIAMethodResources(int? impactCategoryID = null);
        IEnumerable<LCIAFactorResource> GetLCIAFactors(int lciaMethodId);
        IEnumerable<ScenarioResource> GetScenarios();
        IEnumerable<ScenarioResource> GetScenarios(int ScenarioGroupID);
        IEnumerable<FragmentResource> GetFragmentResources();
        FragmentResource GetFragmentResource(int fragmentID);
        IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FlowResource> GetFlows(int flowtypeID);
        IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID);
        IEnumerable<FragmentStageResource> GetStagesByFragment(int fragmentID = 0);
        IEnumerable<ProcessFlowResource> GetProcessFlows(int processID);
        IEnumerable<FlowPropertyResource> GetFlowProperties();
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID);
        LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID, int scenarioGroupID = 1);
        IEnumerable<ProcessResource> GetProcesses(int? flowTypeID = null);
        IEnumerable<ImpactCategoryResource> GetImpactCategories();
        LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FlowTypeResource> GetFlowTypes();
        void ClearNodeCacheByScenario(int scenarioId);
        void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);
        void ClearScoreCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);
        void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int lciaMethodId = 0);
        IEnumerable<ParamResource> GetParams(int scenarioId);
        void AddScenario(string addScenarioJSON);
        void UpdateScenario(string updateScenarioJSON);
        void DeleteScenario(string deleteScenarioJSON);
        void DeleteParam(string deleteParamJSON);
        void AddParam(string addParamJSON);
        void UpdateParam(string updateParamJSON);
    }
}
