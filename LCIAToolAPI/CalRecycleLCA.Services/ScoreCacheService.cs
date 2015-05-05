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

        public void ClearScoreCacheByScenario(int scenarioId)
        {
            _repository.ClearScoreCacheByScenario(scenarioId);
        }

        public void ClearScoreCacheByScenario(int scenarioId, List<int> fragmentFlows)
        {
            _repository.ClearScoreCacheByScenario(scenarioId, fragmentFlows);
        }
        
        /* public void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _repository.ClearScoreCacheByScenarioAndFragment(scenarioId, fragmentId);
        }
        */

        public void ClearScoreCacheForFragments(int scenarioId)
         {
             _repository.ClearScoreCacheForFragments(scenarioId);
         }

        public void ClearScoreCacheForParentFragments(List<int> fragmentIds, int scenarioId)
        {
            _repository.ClearScoreCacheForParentFragments(fragmentIds, scenarioId);
        }

        public void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId, int lciaMethodID = 0)
        {
            _repository.ClearScoreCacheByScenarioAndLCIAMethod(scenarioId, lciaMethodID);
        }

        public IEnumerable<ScoreCache> GetScenarioCaches(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
         {
             return _repository.GetScenarioCaches(fragmentId, scenarioId);
         }
        
        public IEnumerable<ScoreCache> GetFragmentFlowCaches(int fragmentFlowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.GetFragmentFlowCaches(fragmentFlowId, scenarioId);
        }

    }

}
