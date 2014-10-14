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
    public interface IScoreCacheService : IService<ScoreCache>
    {
    }

    public class ScoreCacheService : Service<ScoreCache>, IScoreCacheService
    {
        public ScoreCacheService(IRepositoryAsync<ScoreCache> repository)
            : base(repository)
        {

        }
    }

}
