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
        IEnumerable<FragmentResource> GetFragmentResources(int? fragmentId = null);
        IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FlowResource> GetFlow(int flowId);
        IEnumerable<FlowResource> GetFlows(int flowtypeID = 0);
        IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID);
        IEnumerable<FragmentStageResource> GetStagesByFragment(int fragmentID = 0);
        IEnumerable<ProcessFlowResource> GetProcessFlows(int processID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FlowPropertyResource> GetFlowProperties();
        IEnumerable<FlowPropertyMagnitude> GetFlowFlowProperties(int flowId);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID);
        LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID, int scenarioGroupID = 1);
        IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllMethods(int fragmentID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAResultResource> GetFragmentSensitivity(int fragmentId, int paramId);
        IEnumerable<LCIAResultResource> GetFragmentSensitivity(int fragmentId, ParamResource param);
        IEnumerable<ProcessResource> GetProcesses(int flowTypeID = 0);
        IEnumerable<ProcessResource> GetProcess(int processId);
        IEnumerable<ImpactCategoryResource> GetImpactCategories();
        LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<LCIAResultResource> GetProcessLCIAResults(int processID, int scenarioID = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FlowTypeResource> GetFlowTypes();
        IEnumerable<ParamResource> GetParams(int scenarioId);
        int AddScenario(ScenarioResource inScenario, int scenarioGroupId);
        bool UpdateScenario(int scenarioId, ScenarioResource inScenario);
        void DeleteScenario(int scenarioId);
        //void DeleteParam(string deleteParamJSON);
        IEnumerable<ParamResource> AddParam(int scenarioId, ParamResource postParam);
        IEnumerable<ParamResource> UpdateParams(int scenarioId, IEnumerable<ParamResource> putParams);
        IEnumerable<ParamResource> UpdateParam(int scenarioId, int paramId, ParamResource postParam);
        bool DeleteParam(int scenarioId, int paramId);
        //void UpdateParam(string updateParamJSON);
    }
}
