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
    public static class ProcessRepository
    {
        public static bool CheckPrivacy(this IRepositoryAsync<Process> repository,
            int processID)
	    {
	        return (repository.GetRepository<Process>()
                .Query(p => p.ProcessID == processID)
                .Include(p => p.ILCDEntity.DataSource)
                .Select(p => p.ILCDEntity.DataSource.VisibilityID).First() == 2);

	    }
    }
}
