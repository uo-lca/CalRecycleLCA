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

        public static FragmentNodeResource GetFragmentNodeSubFragmentId(this IRepositoryAsync<FragmentNodeFragment> repository, int fragmentFlowId)
        {
            var fragmentNodeFragment = repository.GetRepository<FragmentNodeFragment>().Queryable()
               .Where(x => x.FragmentFlowID == fragmentFlowId)
                .Select(a => new FragmentNodeResource
                {
                    SubFragmentID = a.SubFragmentID,
                    TermFlowID = a.FlowID
                }).FirstOrDefault();

            return fragmentNodeFragment;
        }
    }
}
