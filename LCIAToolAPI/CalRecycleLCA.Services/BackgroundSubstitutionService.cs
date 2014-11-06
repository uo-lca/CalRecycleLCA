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
    public interface IBackgroundSubstitutionService : IService<BackgroundSubstitution>
    {
    }

    public class BackgroundSubstitutionService : Service<BackgroundSubstitution>, IBackgroundSubstitutionService
    {
        public BackgroundSubstitutionService(IRepositoryAsync<BackgroundSubstitution> repository)
            : base(repository)
        {

        }
    }
}
