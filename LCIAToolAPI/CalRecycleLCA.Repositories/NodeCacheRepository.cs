using LcaDataModel;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Repositories
{
    public static class NodeCacheRepository
    {
        public static void ClearNodeCacheByScenario(this IRepositoryAsync<NodeCache> repository, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            var nodeCaches = repository.GetRepository<NodeCache>().Queryable().Where(nc => nc.ScenarioID == scenarioId).ToList();

            nodeCaches.ForEach(x =>
                {
                    repository.Delete(x.NodeCacheID);
                });
        }

        public static void ClearNodeCacheByScenarioAndFragment(this IRepositoryAsync<NodeCache> repository, int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            //get nodeCaches by scenarioId
            var nodeCaches = repository.GetRepository<NodeCache>().Queryable().Where(nc => nc.ScenarioID == scenarioId);
            //get fragmentFlows by fragmentId
            var fragmentFlows = repository.GetRepository<FragmentFlow>().Queryable().Where(ff => ff.FragmentID == fragmentId);

            //join nodeCaches and fragmentFlows on FragmentFlowID to get only the NodeCache data that corresponds to the params passed
            var query = nodeCaches
            .Join(fragmentFlows,
            nc => nc.FragmentFlowID,
            ff => ff.FragmentFlowID,
            (nc, ff) => new { NodeCache = nc, FragmentFlow = ff }).ToList();

            //delete each of the returned NodeCacheIDs from the NodeCache table
            query.ForEach(x =>
            {
                repository.Delete(x.NodeCache.NodeCacheID);
            });
        }

        /// <summary>
        /// Uses the fragment's reference flow as an indicator that the fragment has been traversed
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public static bool IsCached(this IRepositoryAsync<NodeCache> repository, int fragmentId, int scenarioId)
        {
            var refFlow = repository.GetRepository<FragmentFlow>().Queryable()
                .Where(ff => ff.FragmentID == fragmentId)
                .Where(ff => ff.ParentFragmentFlowID == null)
                .Select(ff => ff.FragmentFlowID).First();
            return (repository.Query(k => k.FragmentFlowID == refFlow && k.ScenarioID == scenarioId)
                .Select().ToList().Count() > 0);
        }
    }
}
