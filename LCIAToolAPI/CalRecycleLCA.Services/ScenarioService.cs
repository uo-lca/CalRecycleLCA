using LcaDataModel;
using Repository.Pattern.Repositories;
using Repository.Pattern.Infrastructure;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalRecycleLCA.Repositories;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public interface IScenarioService : IService<Scenario>
    {
        ScenarioResource GetResource(Scenario s);
        ScenarioResource GetResource(int scenarioId);
        
        bool IsStale(int ScenarioId);
        void MarkStale(int ScenarioId);
        void UnMarkStale(int ScenarioId);
        void CloneScenarioElements(int newScenarioId, int refScenarioId);

        Scenario NewScenario(ScenarioResource scenario);
        Scenario UpdateScenarioFlow(int scenarioId, ScenarioResource scenario, ref CacheTracker cacheTracker);
        Scenario UpdateScenarioDetails(int scenarioId, ScenarioResource scenario);

        int? PublishScenario(int scenarioId, int targetGroupId = ScenarioGroup.BASE_SCENARIO_GROUP);
        void DeleteScenario(int scenarioId);

    }

    public class ScenarioService : Service<Scenario>, IScenarioService
    {
        private IRepositoryAsync<Scenario> _repository;

        public ScenarioService(IRepositoryAsync<Scenario> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public ScenarioResource GetResource(Scenario s)
        {
            return new ScenarioResource
            {
                ScenarioID = s.ScenarioID,
                ScenarioGroupID = s.ScenarioGroupID,
                Name = s.Name,
                TopLevelFragmentID = s.TopLevelFragmentID,
                ActivityLevel = s.ActivityLevel,
                ReferenceFlowID = s.FlowID,
                ReferenceDirection = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)s.DirectionID)
            };
        }

        public ScenarioResource GetResource(int scenarioId)
        {
            return _repository.Query(k => k.ScenarioID == scenarioId).Select().Select(k => GetResource(k)).FirstOrDefault();
        }

        public void MarkStale(int scenarioId)
        {
            _repository.MarkStale(scenarioId);
        }

        public void UnMarkStale(int scenarioId)
        {
            _repository.UnMarkStale(scenarioId);
        }

        public bool IsStale(int scenarioId)
        {
            return _repository.Query(k => k.ScenarioID == scenarioId).Select(k => k.StaleCache).FirstOrDefault();
        }

        public Scenario NewScenario(ScenarioResource post)
        {
            return _repository.PostScenario(post);
        }

        public Scenario UpdateScenarioFlow(int scenarioId, ScenarioResource put, ref CacheTracker cacheTracker)
        {
            Scenario scenario = _repository.UpdateScenarioFlow(scenarioId, put, ref cacheTracker);
            return _repository.UpdateScenario(scenario, put);
        }

        public Scenario UpdateScenarioDetails(int scenarioId, ScenarioResource put)
        {
            Scenario scenario = _repository.Query(k => k.ScenarioID == scenarioId).Select().FirstOrDefault();
            return (scenario == null) 
                ? scenario
                : _repository.UpdateScenario(scenario, put);
        }

        public int? PublishScenario(int scenarioId, int targetGroupId = ScenarioGroup.BASE_SCENARIO_GROUP)
        {
            Scenario scenario = _repository.Query(k => k.ScenarioID == scenarioId).Select().FirstOrDefault();
            if (scenario == null)
                return null;
            scenario.ScenarioGroupID = targetGroupId;
            scenario.ObjectState = ObjectState.Modified;
            _repository.Update(scenario);
            return targetGroupId;
        }

        public void DeleteScenario(int scenarioId)
        {
            _repository.DeleteScenario(scenarioId);
        }

        public void CloneScenarioElements(int newScenarioId, int refScenarioId)
        {
            _repository.CloneScenarioElements(newScenarioId, refScenarioId);
        }

    }
}

