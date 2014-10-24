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
    public static class FragmentFlowRepository
    {
        public static FragmentFlowResource FragmentFlowExtension(this IRepositoryAsync<FragmentFlow> repository, int fragmentFlowId, int nodeTypeId)
        {
            int? processId=0;
            int? subFragmentId=0;
            int? termFlowId=0;

            if (nodeTypeId == 1)
            {
                var process = repository.GetRepository<FragmentNodeProcess>().Queryable()
                    .Where(x => x.FragmentFlowID == fragmentFlowId)
                    .Select(a => new 
                    {
                        ProcessID = a.ProcessID, 
                        FlowID = a.FlowID
                    });

                processId = process
                      .FirstOrDefault()
                      .ProcessID;

                termFlowId = process
                     .FirstOrDefault()
                     .FlowID;

            }
            else if (nodeTypeId == 2)
            {
                 var fragment = repository.GetRepository<FragmentNodeFragment>().Queryable()
                    .Where(x => x.FragmentFlowID == fragmentFlowId)
                     .Select(a => new
                     {
                         SubFragmentID = a.SubFragmentID,
                         FlowID = a.FlowID
                     });

                 subFragmentId = fragment
                       .FirstOrDefault()
                       .SubFragmentID;
                 termFlowId = fragment
                     .FirstOrDefault()
                     .FlowID;
            }

            var fragmentFlowResource = new FragmentFlowResource
            {
                ProcessID = processId,
                SubFragmentID = subFragmentId
                //TermFlowID = termFlowId
            };

            return fragmentFlowResource;

        }
    }
}
