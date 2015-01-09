using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public interface IScenarioGroupService : IService<ScenarioGroup>
    {
        bool CanGet(HttpRequestContext request);
        bool CanAlter(HttpRequestContext request);
        bool CanAlter(HttpRequestContext request, out int authorizedGroup);
        int? CheckAuthorizedGroup(HttpRequestContext request);
        IEnumerable<ScenarioGroupResource> AuthorizedGroups(HttpRequestContext request);
    }

    public class ScenarioGroupService : Service<ScenarioGroup>, IScenarioGroupService
    {
        private IRepositoryAsync<ScenarioGroup> _repository;

        public ScenarioGroupService(IRepositoryAsync<ScenarioGroup> repository)
            : base(repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returns true if the authorization data permits the user to get the named scenario.
        /// </summary>
        /// <param name="request">HttpRequestContext</param>
        /// <returns>bool</returns>
        public bool CanGet(HttpRequestContext request)
        {
            bool auth=false;
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            int? scenarioId = Convert.ToInt32(request.RouteData.Values["scenarioId"]);
            var desiredGroup = _repository.GetRepository<Scenario>()
                .Query(k => k.ScenarioID == scenarioId)
                .Select(k => k.ScenarioGroupID).FirstOrDefault();
            if (desiredGroup == 0)
                return false; // scenario does not exist- so no, you can't get it
            if (desiredGroup == ScenarioGroup.BASE_SCENARIO_GROUP)
                auth=true;
            else
                if (desiredGroup == CheckAuthorizedGroup(request))
                    auth=true;
            return auth;
        }

        public bool CanAlter(HttpRequestContext request)
        {
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            int? scenarioId = Convert.ToInt32(request.RouteData.Values["scenarioId"]);
            int? desiredGroup = _repository.GetRepository<Scenario>()
                .Query(k => k.ScenarioID == scenarioId)
                .Select(k => k.ScenarioGroupID).FirstOrDefault();
            if (desiredGroup == 0)
                return false; // scenario does not exist- so no, you can't alter it
            return (desiredGroup == CheckAuthorizedGroup(request));
        }

        public bool CanAlter(HttpRequestContext request, out int authGroup)
        {
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            int? scenarioId = Convert.ToInt32(request.RouteData.Values["scenarioId"]);
            authGroup = (int)CheckAuthorizedGroup(request);
            return (_repository.GetRepository<Scenario>()
                .Query(k => k.ScenarioID == scenarioId)
                .Select(k => k.ScenarioGroupID).First() == authGroup);
        }
        
        public int? CheckAuthorizedGroup(HttpRequestContext request)
        {
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            return _repository.Queryable().Where(k => k.Secret == authString).Select(k => k.ScenarioGroupID).FirstOrDefault();
        }

        public IEnumerable<ScenarioGroupResource> AuthorizedGroups(HttpRequestContext request)
        {
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            return _repository.Queryable().Where(k => k.Secret == authString || k.ScenarioGroupID == 1).Select(k => new ScenarioGroupResource()
            {
                Name = k.Name,
                ScenarioGroupID = k.ScenarioGroupID
            });
        }
    }
}

