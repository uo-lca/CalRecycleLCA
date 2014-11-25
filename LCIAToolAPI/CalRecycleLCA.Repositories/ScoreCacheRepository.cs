using LcaDataModel;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Repositories
{
    public static class ScoreCacheRepository
    {
        public static void ClearScoreCacheByScenario(this IRepositoryAsync<ScoreCache> repository, int scenarioId = 0)
        {
            var scoreCaches = repository.GetRepository<ScoreCache>().Queryable().Where(sc => sc.ScenarioID == scenarioId).ToList();

            scoreCaches.ForEach(x =>
            {
                repository.Delete(x.ScoreCacheID);
            });
        }

        public static void ClearScoreCacheByScenarioAndFragment(this IRepositoryAsync<ScoreCache> repository, int scenarioId = 0, int fragmentId = 0)
        {
            //get scoreCaches by scenarioId
            var scoreCaches = repository.GetRepository<ScoreCache>().Queryable().Where(sc => sc.ScenarioID == scenarioId);
            //get fragmentFlows by fragmentId
            var fragmentFlows = repository.GetRepository<FragmentFlow>().Queryable().Where(ff => ff.FragmentID == fragmentId);

            //join nodeCaches and fragmentFlows on FragmentFlowID to get only the ScoreCache data that corresponds to the params passed
            var query = scoreCaches
            .Join(fragmentFlows,
            sc => sc.FragmentFlowID,
            ff => ff.FragmentFlowID,
            (sc, ff) => new { ScoreCache = sc, FragmentFlow = ff }).ToList();

            //delete each of the returned ScoreCacheIDs from the ScoreCache table
            query.ForEach(x =>
            {
                repository.Delete(x.ScoreCache.ScoreCacheID);
            });
        }

        public static void ClearScoreCacheByScenarioAndLCIAMethod(this IRepositoryAsync<ScoreCache> repository, int scenarioId = 0, int lciaMethodID = 0)
        {
            //get scoreCaches by scenarioId and LCIAMethod
            var scoreCaches = repository.GetRepository<ScoreCache>().Queryable()
                .Where(sc => sc.ScenarioID == scenarioId && sc.LCIAMethodID == lciaMethodID )
                .ToList();

            //delete each of the returned ScoreCacheIDs from the ScoreCache table
            scoreCaches.ForEach(x =>
            {
                repository.Delete(x.ScoreCacheID);
            });
        }
    }
}
