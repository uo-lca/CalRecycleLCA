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
    public interface IParamService : IService<Param>
    {
    }

    public class ParamService : Service<Param>, IParamService
    {
        public ParamService(IRepositoryAsync<Param> repository)
            : base(repository)
        {
                    
        }
    }
}
