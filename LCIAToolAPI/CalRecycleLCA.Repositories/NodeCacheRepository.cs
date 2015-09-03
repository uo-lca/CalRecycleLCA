using LcaDataModel;
using Repository.Pattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace CalRecycleLCA.Repositories
{
    public static class NodeCacheRepository
    {
        public static void ClearNodeCacheByScenario(this IRepositoryAsync<NodeCache> repository, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            var nodeCaches = repository.GetRepository<NodeCache>().Queryable().Where(nc => nc.ScenarioID == scenarioId).ToList();

            nodeCaches.ForEach(x =>
                {
                    repository.Delete(x);
                });
        }

        public static void ClearNodeCacheByScenarioAndFragments(this IRepositoryAsync<NodeCache> repository, 
            List<int> fragmentIds, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            var nodeCaches = repository.GetRepository<NodeCache>().Queryable()
                .Where(nc => nc.ScenarioID == scenarioId)
                .Where(nc => fragmentIds.Contains(nc.FragmentFlow.FragmentID))
                .ToList();

            //delete each of the returned NodeCacheIDs from the NodeCache table
            nodeCaches.ForEach(x =>
            {
                repository.Delete(x);
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

        public static IEnumerable<FlowNodeModel> GetLCIAFlows(this IRepository<NodeCache> repository, 
            int fragmentId, int scenarioId)
        {
            return repository.Queryable().Where(nc => nc.ScenarioID == scenarioId)
                .Where(nc => nc.FragmentFlow.FragmentID == fragmentId)
                .Where(nc => nc.FragmentFlow.NodeTypeID != 3)
                .Select(nc => new FlowNodeModel()
                {
                    FragmentFlowID = nc.FragmentFlowID,
                    ScenarioID = nc.ScenarioID, 
                    NodeTypeID = nc.ILCDEntityID == null ? 5 :
                                    (nc.ILCDEntity.DataType.Name == "Process" ? 1 : 2),
                    ProcessID = nc.ILCDEntity.Processes.Select(a => a.ProcessID).FirstOrDefault(),
                    SubFragmentID = nc.ILCDEntity.Fragments.Select(a => a.FragmentID).FirstOrDefault()
                });
        }
    }
}
