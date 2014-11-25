using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using CalRecycleLCA.Repositories;

namespace CalRecycleLCA.Services
{
    public class ScoreCacheService : Service<ScoreCache>, IScoreCacheService
    {
         private readonly IRepositoryAsync<ScoreCache> _repository;

         public ScoreCacheService(IRepositoryAsync<ScoreCache> repository)
            : base(repository)
        {
            _repository = repository; 
        }

        public void ClearScoreCacheByScenario(int scenarioId = 0)
        {
            _repository.ClearScoreCacheByScenario(scenarioId);
        }

        public void ClearScoreCacheByScenarioAndFragment(int scenarioId = 0, int fragmentId = 0)
        {
            _repository.ClearScoreCacheByScenarioAndFragment(scenarioId, fragmentId);
        }

        public void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId = 0, int lciaMethodID = 0)
        {
            _repository.ClearScoreCacheByScenarioAndLCIAMethod(scenarioId, lciaMethodID);
        }
    }

}
