using Data;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FragmentTraversal
    {
        private readonly IFlowService _flowService;
        int fragmentId = 85;

        public FragmentTraversal(IFlowService flowService)
        {

            if (flowService == null)
            {
                throw new ArgumentNullException("flowService is null");
            }

            _flowService = flowService;


        }

        //get fragment flows by fragment id
        public IEnumerable<FragmentFlow> GetFragmentFlows(int fragmentId)
        {
            var unitOfWork = new UnitOfWork();
            var flowsByFragmentId = unitOfWork.Repository<FragmentFlow>().GetFragmentFlows(fragmentId);
            return flowsByFragmentId;
        }

        public void ApplyDependencyParam(int scenarioId = 0)
        {
            var unitOfWork = new UnitOfWork();

            var fragmentFlows = GetFragmentFlows(fragmentId);

            //get scenario specific params
            var scenarioParamsByScenarioId = unitOfWork.Repository<ScenarioParam>().GetDependencyParams(scenarioId);


    //        var source = fragmentFlows.GroupJoin(
    //scenarioParamsByScenarioId,
    //p => p.FragmentFlowID,
    //c => c.FragmentFlowID,
    //(p, g) => g
    //    .Select(c => new { PID = c.Value, CID = c.ParamID, Text = c.FragmentFlowID })
    //    .DefaultIfEmpty(new { PID = p.FragmentFlowID, CID = -1, Text = "[[Empty]]" }))
    //.SelectMany(g => g);
        }



    }
}
