using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IProcessService : IService<Process>
    {
    }

    public class ProcessService : Service<Process>, IProcessService
    {
        public ProcessService(IRepositoryAsync<Process> repository)
            : base(repository)
        {

        }
    }
}
