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
        public static FragmentNodeResource GetFragmentNodeProcessId(this IRepositoryAsync<FragmentNodeProcess> repository, int fragmentFlowId)
        {
            var fragmentNodeProcess = repository.GetRepository<FragmentNodeProcess>().Queryable()
               .Where(x => x.FragmentFlowID == fragmentFlowId)
                .Select(a => new FragmentNodeResource
                {
                    ProcessID = a.ProcessID,
                    TermFlowID = a.FlowID
                }).FirstOrDefault();

            return fragmentNodeProcess;
        }
    }
}
