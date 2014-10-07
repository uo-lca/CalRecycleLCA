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
    public interface IProcessFlowService : IService<ProcessFlow> 
    { 
    }

    public class ProcessFlowService : Service<ProcessFlow>, IProcessFlowService
    {
        public ProcessFlowService(IRepositoryAsync<ProcessFlow> repository) : base(repository)
        {
                    
        }
    }
}
