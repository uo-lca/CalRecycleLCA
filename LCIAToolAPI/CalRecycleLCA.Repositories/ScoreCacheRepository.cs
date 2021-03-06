﻿using LcaDataModel;
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
        public static void ClearScoreCacheByScenario(this IRepositoryAsync<ScoreCache> repository, int scenarioId)
        {
            var scoreCaches = repository.Queryable().Where(sc => sc.ScenarioID == scenarioId).ToList();

            scoreCaches.ForEach(x =>
            {
                repository.Delete(x);
            });
        }

        /// <summary>
        /// Like above, but only clears cache values for the named FragmentFlowIDs
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="scenarioId"></param>
        /// <param name="fragmentFlows"></param>
        public static void ClearScoreCacheByScenario(this IRepositoryAsync<ScoreCache> repository, int scenarioId,
            List<int> fragmentFlows)
        {
            fragmentFlows = fragmentFlows.Distinct().ToList();
            var scoreCaches = repository.Queryable().Where(sc => sc.ScenarioID == scenarioId)
                .Where(sc => fragmentFlows.Contains(sc.FragmentFlowID))
                .ToList();

            scoreCaches.ForEach(x =>
                {
                    repository.Delete(x);
                });
        }

        /*
        public static void ClearScoreCacheByScenarioAndFragment(this IRepositoryAsync<ScoreCache> repository, int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
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

         * */

        public static void ClearScoreCacheForFragments(this IRepositoryAsync<ScoreCache> repository, int scenarioId)
        {
            var scoreCaches = repository.GetRepository<ScoreCache>().Queryable()
                .Where(sc => sc.ScenarioID == scenarioId)
                .Where(sc => sc.FragmentFlow.NodeTypeID == 2)
                .ToList();

            scoreCaches.ForEach(x =>
                {
                    repository.Delete(x);
                });
        }

        public static void ClearScoreCacheForParentFragments(this IRepositoryAsync<ScoreCache> repository, List<int> fragmentIds, int scenarioId)
        {
            var scoreCaches = repository.GetRepository<NodeCache>().Queryable()
                .Where(nc => nc.ScenarioID == scenarioId)
                .Where(nc => nc.ILCDEntity.DataType.Name == "Fragment")
                .Join(repository.Queryable().Where(sc => sc.ScenarioID == scenarioId),
                    nc => nc.FragmentFlowID,
                    sc => sc.FragmentFlowID,
                    (nc,sc) => new {sc})
                .Where(j => fragmentIds.Contains(j.sc.FragmentFlow.FragmentID))
                .Select(j => j.sc)
                .ToList();

            scoreCaches.ForEach(x =>
            {
                repository.Delete(x);
            });
        }
        
        public static void ClearScoreCacheByScenarioAndLCIAMethod(this IRepositoryAsync<ScoreCache> repository, int scenarioId, int lciaMethodID)
        {
            //get scoreCaches by scenarioId and LCIAMethod
            var scoreCaches = repository.GetRepository<ScoreCache>().Queryable()
                .Where(sc => sc.ScenarioID == scenarioId && sc.LCIAMethodID == lciaMethodID )
                .ToList();

            //delete each of the returned ScoreCacheIDs from the ScoreCache table
            scoreCaches.ForEach(x =>
            {
                repository.Delete(x);
            });
        }

        public static IEnumerable<ScoreCache> GetScenarioCaches(this IRepositoryAsync<ScoreCache> repository, int fragmentId, int scenarioId)
        {
            return repository.Queryable()
                .Where(sc => sc.ScenarioID == scenarioId)
                .Where(sc => sc.FragmentFlow.FragmentID == fragmentId);
        }

        public static IEnumerable<ScoreCache> GetFragmentFlowCaches(this IRepositoryAsync<ScoreCache> repository, int fragmentFlowId, int scenarioId)
        {
            return repository.Queryable()
                .Where(sc => sc.ScenarioID == scenarioId)
                .Where(sc => sc.FragmentFlowID == fragmentFlowId);
        }
    }
}
