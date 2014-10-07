using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Repository.Pattern.Repositories;

namespace CalRecycleLCA.Services
{
    public interface IProcessEmissionParamService : IService<ProcessEmissionParam>
    {
    }

    public class ProcessEmissionParamService : Service<ProcessEmissionParam>, IProcessEmissionParamService
    {
        public ProcessEmissionParamService(IRepositoryAsync<ProcessEmissionParam> repository)
            : base(repository)
        {
                    
        }
    }
}
