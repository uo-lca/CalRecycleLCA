using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalRecycleLCA.Repositories;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public interface IProcessService : IService<Process>
    {
        bool IsPrivate(int processID);
        IEnumerable<ProcessResource> GetProcesses(int flowTypeId = 0);
        IEnumerable<ProcessResource> GetProcess(int processId);
    }

    public class ProcessService : Service<Process>, IProcessService
    {
        private readonly IRepositoryAsync<Process> _repository;
        public ProcessService(IRepositoryAsync<Process> repository)
            : base(repository)
        {
            _repository = repository;

        }
        
        public bool IsPrivate(int processID)
        {
            return _repository.CheckPrivacy(processID);
        }

        public IEnumerable<ProcessResource> GetProcess(int processId)
        {
            return _repository.GetProcess(processId);
        }
        public IEnumerable<ProcessResource> GetProcesses(int flowTypeId = 0)
        {
            IEnumerable<ProcessResource> pData = _repository.GetProcesses();
            if (flowTypeId == 2)
                return pData.Where(p => p.hasElementaryFlows == true);
            else
                return pData;
        }
    }
}

