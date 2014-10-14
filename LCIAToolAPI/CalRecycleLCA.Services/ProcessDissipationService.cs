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
    public interface IProcessDissipationService : IService<ProcessDissipation>
    {
    }

    public class ProcessDissipationService : Service<ProcessDissipation>, IProcessDissipationService
    {
        public ProcessDissipationService(IRepositoryAsync<ProcessDissipation> repository)
            : base(repository)
        {
                    
        }
    }
}
