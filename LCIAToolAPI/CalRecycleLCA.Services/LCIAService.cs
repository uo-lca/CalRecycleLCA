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
    public interface ILCIAService : IService<LCIA>
    {
    }

    public class LCIAService : Service<LCIA>, ILCIAService
    {
        public LCIAService(IRepositoryAsync<LCIA> repository)
            : base(repository)
        {
                    
        }
    }
}
