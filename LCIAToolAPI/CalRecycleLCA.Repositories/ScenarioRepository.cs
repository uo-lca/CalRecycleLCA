﻿using System;
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
                DirectionID = post.ReferenceDirectionID
            };
            scenario.ObjectState = ObjectState.Added;
            repository.Insert(scenario);
            return scenario;
        }

        public static Scenario UpdateScenarioFlow(this IRepositoryAsync<Scenario> repository, 
            int scenarioId, ScenarioResource put)
        {
            Scenario scenario = repository.Query(k => k.ScenarioID == scenarioId).Select().First();
            scenario.TopLevelFragmentID = put.TopLevelFragmentID;
            scenario.FlowID = put.ReferenceFlowID;
            scenario.DirectionID = put.ReferenceDirectionID;
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
    }
}