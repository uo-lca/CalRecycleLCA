using LcaDataModel;
using Repository.Pattern.Repositories;
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
        Scenario NewScenario(ScenarioResource scenario);
        Scenario UpdateScenarioFlow(int scenarioId, ScenarioResource scenario, ref CacheTracker cacheTracker);
        Scenario UpdateScenarioDetails(int scenarioId, ScenarioResource scenario);
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
            Scenario scenario = _repository.Query(k => k.ScenarioID == scenarioId).Select().First();
            return _repository.UpdateScenario(scenario, put);
        }

        public void DeleteScenario(int scenarioId)
        {
            _repository.DeleteScenario(scenarioId);
        }
    }
}

