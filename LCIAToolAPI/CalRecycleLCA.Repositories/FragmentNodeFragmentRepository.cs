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
    public static class FragmentNodeFragmentRepository
    {
        public static FragmentNodeResource GetFragmentNodeSubFragment(this IRepository<FragmentNodeFragment> repository, 
									int fragmentFlowId, int scenarioId)
        {
            var fragmentNode = repository.GetRepository<FragmentNodeFragment>()
		        .Query(x => x.FragmentFlowID == fragmentFlowId)
		        .Select(a => new FragmentNodeResource
		        {
		            RefID = a.FragmentNodeFragmentID,
        		    ScenarioID = 0,
		            SubFragmentID = a.SubFragmentID,
        		    TermFlowID = a.FlowID
		        }).First();
	        
    	    if (scenarioId != 0)
	        {
                var substituteNode = repository.GetRepository<FragmentSubstitution>()
                    .Query(x => x.FragmentNodeFragmentID == fragmentNode.RefID
                            && x.ScenarioID == scenarioId)
                    .Select(a => new FragmentNodeResource
                    {
                        RefID = a.FragmentNodeFragmentID,
                        ScenarioID = a.ScenarioID,
	                    SubFragmentID = a.SubFragmentID,
                        TermFlowID = fragmentNode.TermFlowID
                    }).FirstOrDefault();
                if (substituteNode != null)
                    fragmentNode = substituteNode;
            }
            fragmentNode.NodeTypeID = 2;
            return fragmentNode;
        }
    }
}
