using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Services {
    public interface IResourceServiceFacade {
        IEnumerable<LCIAMethodResource> GetLCIAMethodResources();
        IEnumerable<FragmentResource> GetFragmentResources();
        FragmentResource GetFragmentResource(int fragmentID);
        IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID);
        IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID);
        IEnumerable<ProcessResource> GetProcesses();
    }
}
