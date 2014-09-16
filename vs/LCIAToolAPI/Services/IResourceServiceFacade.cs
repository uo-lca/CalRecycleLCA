using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Services {
    public interface IResourceServiceFacade {
        IEnumerable<LCIAMethodResource> GetLCIAMethodResources(int? impactCategoryID = null);
        IEnumerable<FragmentResource> GetFragmentResources();
        FragmentResource GetFragmentResource(int fragmentID);
        IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID);
        IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID);
        IEnumerable<FlowResource> GetFlowsByProcess(int processID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID);
        IEnumerable<ProcessResource> GetProcesses(int? flowTypeID = null);
        IEnumerable<ImpactCategoryResource> GetImpactCategories();
        IEnumerable<LCIAResultResource> GetLCIAResultResources(int processID, int lciaMethodID, int? scenarioID = 1);
        IEnumerable<FlowTypeResource> GetFlowTypes();
    }
}
