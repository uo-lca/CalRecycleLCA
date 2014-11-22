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
    public interface IFragmentStageService : IService<FragmentStage>
    {
    }

    public class FragmentStageService : Service<FragmentStage>, IFragmentStageService
    {
        public FragmentStageService(IRepositoryAsync<FragmentStage> repository)
            : base(repository)
        {

        }
    }
}

