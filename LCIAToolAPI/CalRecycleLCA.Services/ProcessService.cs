using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalRecycleLCA.Repositories;

namespace CalRecycleLCA.Services
{
    public interface IProcessService : IService<Process>
    {
        bool IsPrivate(int processID);
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
    }
}

