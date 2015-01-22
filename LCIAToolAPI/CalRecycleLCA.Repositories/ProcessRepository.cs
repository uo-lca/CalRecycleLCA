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
        private static ProcessResource ToResource(this IRepositoryAsync<Process> repository, Process p)
        {
            return new ProcessResource
            {
                ProcessID = p.ProcessID,
                Name = p.Name,
                Geography = p.Geography,
                //ProcessTypeID = TransformNullable(p.ProcessTypeID, "Process.ProcessTypeID"),
                ReferenceTypeID = p.ReferenceTypeID,
                ReferenceFlowID = p.ReferenceFlowID,
                ReferenceYear = p.ReferenceYear,
                UUID = p.ILCDEntity.UUID,
                Version = p.ILCDEntity.Version,
                DataSource = p.ILCDEntity.DataSource.Name,
                isPrivate = (p.ILCDEntity.DataSource.VisibilityID == 2),
                hasElementaryFlows = repository.GetRepository<ProcessFlow>().Queryable()
                    .Where(pf => pf.ProcessID == p.ProcessID)
                    .Where(pf => pf.Flow.FlowTypeID == 2).Any()
            };
        }


        public static bool CheckPrivacy(this IRepositoryAsync<Process> repository,
            int processID)
	    {
            return (repository.GetRepository<Process>()
                .Query(p => p.ProcessID == processID)
                .Include(p => p.ILCDEntity.DataSource)
                .Select(p => p.ILCDEntity.DataSource.VisibilityID).First() == 2);
	    }

        public static IEnumerable<ProcessResource> GetProcess(this IRepositoryAsync<Process> repository,
            int processId)
        {
            return repository.Query(p => p.ProcessID == processId)
                .Include(p => p.ILCDEntity.DataSource)
                .Select()
                .Select(p => repository.ToResource(p)).ToList();
        }

        public static IEnumerable<ProcessResource> GetProcesses(this IRepositoryAsync<Process> repository)
        {
            return repository.Query()
                .Include(p => p.ILCDEntity.DataSource)
                .Select()
                .Select(p => repository.ToResource(p)).ToList();
        }
    }
}
