using LcaDataModel;
using Repository.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Entities.Models;

namespace CalRecycleLCA.Repositories
{
    public static class FragmentNodeProcessRepository
    {
        public static FragmentNodeResource GetFragmentNodeProcessId(this IRepositoryAsync<FragmentNodeProcess> repository, 
            int fragmentFlowId, int scenarioID = 0)
        {
            var fragmentNode = repository.GetRepository<FragmentNodeProcess>()
                .Query(x => x.FragmentFlowID == fragmentFlowId)
                .Select(a => new FragmentNodeResource
                    {
                        FragmentNodeID = a.FragmentNodeProcessID,
                        ScenarioID = 0,
                        ProcessID = a.ProcessID,
                        TermFlowID = a.FlowID
                    }).First();

            if (scenarioID != 0)
            {
                var substituteNode = repository.GetRepository<ProcessSubstitution>()
                    .Query(x => x.FragmentNodeProcessID == fragmentNode.FragmentNodeID
                      && x.ScenarioID == scenarioID)
                    .Select(a => new FragmentNodeResource
                    {
                        FragmentNodeID = a.FragmentNodeProcessID,
                        ScenarioID = a.ScenarioID,
                        ProcessID = a.ProcessID,
                        TermFlowID = fragmentNode.TermFlowID
                    }).FirstOrDefault();
                if (substituteNode != null)
                    fragmentNode = substituteNode;
            }
            return fragmentNode;
        }
    }
}
