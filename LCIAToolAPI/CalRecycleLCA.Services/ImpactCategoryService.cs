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
    public interface IImpactCategoryService : IService<ImpactCategory>
    {
    }

    public class ImpactCategoryService : Service<ImpactCategory>, IImpactCategoryService
    {
        public ImpactCategoryService(IRepositoryAsync<ImpactCategory> repository)
            : base(repository)
        {

        }
    }
}
