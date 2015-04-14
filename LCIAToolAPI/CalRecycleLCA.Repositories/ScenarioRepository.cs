using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Entities.Models;

namespace CalRecycleLCA.Repositories
{
    public static class ScenarioRepository
    {
        public static Scenario PostScenario(this IRepositoryAsync<Scenario> repository, ScenarioResource post)
        {
            Scenario scenario = new Scenario()
            {
                ScenarioGroupID = post.ScenarioGroupID,
                Name = post.Name,
                ActivityLevel = post.ActivityLevel,
                TopLevelFragmentID = post.TopLevelFragmentID,
                FlowID = post.ReferenceFlowID,
                DirectionID = Convert.ToInt32(Enum.Parse(typeof(DirectionEnum),post.ReferenceDirection)),
                StaleCache = true
            };
            scenario.ObjectState = ObjectState.Added;
            repository.Insert(scenario);
            return scenario;
        }

        public static void MarkStale(this IRepository<Scenario> repository, int scenarioId)
        {
            Scenario scenario = repository.Queryable()
                .Where(k => k.ScenarioID == scenarioId).First();
            scenario.StaleCache = true;
            scenario.ObjectState = ObjectState.Modified;
            repository.Update(scenario);
        }

        public static void UnMarkStale(this IRepository<Scenario> repository, int scenarioId)
        {
            Scenario scenario = repository.Queryable()
                .Where(k => k.ScenarioID == scenarioId).First();
            scenario.StaleCache = false;
            scenario.ObjectState = ObjectState.Modified;
            repository.Update(scenario);
        }

        public static Scenario UpdateScenarioFlow(this IRepositoryAsync<Scenario> repository, 
            int scenarioId, ScenarioResource put, ref CacheTracker cacheTracker)
        {
            Scenario scenario = repository.Query(k => k.ScenarioID == scenarioId).Select().First();
            if (scenario.TopLevelFragmentID != put.TopLevelFragmentID)
            {
                scenario.TopLevelFragmentID = put.TopLevelFragmentID;
                cacheTracker.Recompute = true;
            }
            if (scenario.FlowID != put.ReferenceFlowID)
            {
                scenario.FlowID = put.ReferenceFlowID;
                cacheTracker.NodeCacheStale = true;
            }
            scenario.DirectionID = Convert.ToInt32(Enum.Parse(typeof(DirectionEnum),put.ReferenceDirection));
            return scenario;
        }

        public static Scenario UpdateScenario(this IRepositoryAsync<Scenario> repository,
            Scenario scenario, ScenarioResource put)
        {
            if (put.Name != null)
                scenario.Name = put.Name;
            if (put.ActivityLevel != 0)
                scenario.ActivityLevel = put.ActivityLevel;
            scenario.ObjectState = ObjectState.Modified;
            repository.Update(scenario);
            return scenario;
        }

        public static void DeleteScenario(this IRepositoryAsync<Scenario> repository, int scenarioId)
        {
            Scenario scenario = repository.Query(k => k.ScenarioID == scenarioId).Select().First();
            scenario.ObjectState = ObjectState.Deleted;
            repository.Delete(scenario);
            // hoping entity framework automatically deletes descendents! wouldn't that be cool?
        }

        public static void CloneScenarioElements(this IRepository<Scenario> repository, int newScenarioId, int refScenarioId)
        {
            // need to clone params and substitutions as well-- can I do this with eager query?
            // even this is fairly verbose-- I wonder if there is a better way.....
            var S = repository.Query(k => k.ScenarioID == refScenarioId)
                    .Include(k => k.ProcessSubstitutions)
                    .Include(k => k.FragmentSubstitutions)
                    .Include(k => k.BackgroundSubstitutions)
                    .Select().First();

            var Ps = repository.GetRepository<Param>().Query(k => k.ScenarioID == refScenarioId)
                    .Include(k => k.DependencyParams)
                    .Include(k => k.FlowPropertyParams)
                    .Include(k => k.CompositionParams)
                    .Include(k => k.ProcessDissipationParams)
                    .Include(k => k.ProcessEmissionParams)
                    .Include(k => k.CharacterizationParams)
                    .Select().ToList();

            foreach (var p in Ps)
            {
                p.ScenarioID = newScenarioId;
                p.ObjectState = ObjectState.Added;
                switch (p.ParamTypeID)
                {
                    case 1:
                        {
                            foreach (var dp in p.DependencyParams)
                                dp.ObjectState = ObjectState.Added;
                            break;
                        }
                    case 4:
                        {
                            foreach (var fp in p.FlowPropertyParams)
                                fp.ObjectState = ObjectState.Added;
                            break;
                        }
                    case 5:
                        {
                            foreach (var cp in p.CompositionParams)
                                cp.ObjectState = ObjectState.Added;
                            break;
                        }
                    case 6:
                        {
                            foreach (var pdp in p.ProcessDissipationParams)
                                pdp.ObjectState = ObjectState.Added;
                            break;
                        }
                    case 8:
                        {
                            foreach (var pep in p.ProcessEmissionParams)
                                pep.ObjectState = ObjectState.Added;
                            break;
                        }
                    case 10:
                        {
                            foreach (var cp in p.CharacterizationParams)
                                cp.ObjectState = ObjectState.Added;
                            break;
                        }
                }
            }
            if (Ps.Count > 0)
                repository.GetRepository<Param>().InsertGraphRange(Ps);

            foreach (var ps in S.ProcessSubstitutions)
            {
                ps.ScenarioID = newScenarioId;
                ps.ObjectState = ObjectState.Added;
                repository.GetRepository<ProcessSubstitution>().Insert(ps);
            }

            foreach (var fs in S.FragmentSubstitutions)
            {
                fs.ScenarioID = newScenarioId;
                fs.ObjectState = ObjectState.Added;
                repository.GetRepository<FragmentSubstitution>().Insert(fs);
            }

            foreach (var bs in S.BackgroundSubstitutions)
            {
                bs.ScenarioID = newScenarioId;
                bs.ObjectState = ObjectState.Added;
                repository.GetRepository<BackgroundSubstitution>().Insert(bs);
            }
        }
    }
}
