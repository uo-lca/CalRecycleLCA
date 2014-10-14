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
    public interface IBackgroundService : IService<Background>
    {
    }

    public class BackgroundService : Service<Background>, IBackgroundService
    {
        public BackgroundService(IRepositoryAsync<Background> repository)
            : base(repository)
        {

        }
    }
}
