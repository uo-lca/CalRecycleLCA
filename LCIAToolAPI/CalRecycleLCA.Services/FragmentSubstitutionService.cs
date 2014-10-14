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
    public interface IFragmentSubstitutionService : IService<FragmentSubstitution>
    {
    }

    public class FragmentSubstitutionService : Service<FragmentSubstitution>, IFragmentSubstitutionService
    {
        public FragmentSubstitutionService(IRepositoryAsync<FragmentSubstitution> repository)
            : base(repository)
        {

        }
    }

}
