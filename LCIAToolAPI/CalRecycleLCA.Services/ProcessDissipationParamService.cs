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
    public interface IProcessDissipationParamService : IService<ProcessDissipationParam>
    {
    }

    public class ProcessDissipationParamService : Service<ProcessDissipationParam>, IProcessDissipationParamService
    {
        public ProcessDissipationParamService(IRepositoryAsync<ProcessDissipationParam> repository)
            : base(repository)
        {
                    
        }
    }
}
