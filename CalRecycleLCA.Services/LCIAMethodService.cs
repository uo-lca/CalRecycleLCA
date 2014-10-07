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
  

    public interface ILCIAMethodService : IService<LCIAMethod>
    {
    }

    public class LCIAMethodService : Service<LCIAMethod>, ILCIAMethodService
    {
        public LCIAMethodService(IRepositoryAsync<LCIAMethod> repository)
            : base(repository)
        {

        }
    }
}
