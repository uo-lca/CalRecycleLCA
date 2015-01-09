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
        IEnumerable<ScenarioResource> GetScenarios(int? ScenarioGroupID);
        IEnumerable<FragmentResource> GetFragmentResources();
        FragmentResource GetFragmentResource(int fragmentID);
        IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FlowResource> GetFlow(int flowId);
        IEnumerable<FlowResource> GetFlows(int flowtypeID = 0);
        IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID);
        IEnumerable<FragmentStageResource> GetStagesByFragment(int fragmentID = 0);
        IEnumerable<ProcessFlowResource> GetProcessFlows(int processID);
        IEnumerable<FlowPropertyResource> GetFlowProperties();
        IEnumerable<FlowPropertyMagnitude> GetFlowFlowProperties(int flowId);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID);
        LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID, int scenarioGroupID = 1);
        IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllMethods(int fragmentID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<ProcessResource> GetProcesses(int? flowTypeID = null);
        IEnumerable<ImpactCategoryResource> GetImpactCategories();
        LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAResultResource> GetProcessLCIAResults(int processID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FlowTypeResource> GetFlowTypes();
        void ClearNodeCacheByScenario(int scenarioId);
        //void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);
        void ClearScoreCacheByScenario(int scenarioId);
        //void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0);
        void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId, int lciaMethodId);
        IEnumerable<ParamResource> GetParams(int scenarioId);
        int AddScenario(ScenarioResource inScenario, int scenarioGroupId);
        bool UpdateScenario(int scenarioId, ScenarioResource inScenario);
        void DeleteScenario(int scenarioId);
        //void DeleteParam(string deleteParamJSON);
        IEnumerable<ParamResource> AddParam(int scenarioId, ParamResource postParam);
        IEnumerable<ParamResource> UpdateParam(int scenarioId, int paramId, ParamResource postParam);
        void DeleteParam(int scenarioId, int paramId);
        //void UpdateParam(string updateParamJSON);
    }
}
