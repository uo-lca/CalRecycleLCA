using LcaDataModel;
using Repository;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IDependencyParamService : IService<DependencyParam>
    {
    }

    public class DependencyParamService : Service<DependencyParam>, IDependencyParamService
    {
        public DependencyParamService(IRepositoryAsync<DependencyParam> repository)
            : base(repository)
        {

        }
    }
    
}
