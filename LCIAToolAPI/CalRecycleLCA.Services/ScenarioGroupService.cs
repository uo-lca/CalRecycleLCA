using LcaDataModel;
using Repository.Pattern.Repositories;
using Repository.Pattern.Infrastructure;
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
        bool CanGet(HttpRequestContext request, int scenarioId = 0);
        bool CanAlter(HttpRequestContext request, int scenarioId = 0);
        bool CanAlter(HttpRequestContext request, out int authorizedGroup, int scenarioId = 0);
        int? CheckAuthorizedGroup(HttpRequestContext request);
        ScenarioGroupResource GetResource(int scenarioGroupId);
        IEnumerable<ScenarioGroupResource> AuthorizedGroups(HttpRequestContext request);
        ScenarioGroup AddAuthenticatedScenarioGroup(ScenarioGroupResource postdata);
        ScenarioGroup UpdateScenarioGroup(int scenarioGroupId, ScenarioGroupResource putdata);
        IEnumerable<ScenarioGroupResource> ConfigListAllGroups();
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
        public bool CanGet(HttpRequestContext request, int scenarioId = 0)
        {
            bool auth=false;
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            if (scenarioId == 0) // if not provided as argument, scrape from route data
                scenarioId = Convert.ToInt32(request.RouteData.Values["scenarioId"]);
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

        public bool CanAlter(HttpRequestContext request, int scenarioId = 0)
        {
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            if (scenarioId == 0)
                scenarioId = Convert.ToInt32(request.RouteData.Values["scenarioId"]);
            int? desiredGroup = _repository.GetRepository<Scenario>()
                .Query(k => k.ScenarioID == scenarioId)
                .Select(k => k.ScenarioGroupID).FirstOrDefault();
            if (desiredGroup == 0)
                return false; // scenario does not exist- so no, you can't alter it
            return (desiredGroup == CheckAuthorizedGroup(request));
        }

        public bool CanAlter(HttpRequestContext request, out int authGroup, int scenarioId = 0)
        {
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            if (scenarioId == 0)
                scenarioId = Convert.ToInt32(request.RouteData.Values["scenarioId"]);
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

        public ScenarioGroupResource GetResource(int scenarioGroupId)
        {
            return _repository.Queryable().Where(k => k.ScenarioGroupID == scenarioGroupId).ToList()
                .Select(k => new ScenarioGroupResource
                {
                    Name = k.Name,
                    ScenarioGroupID = k.ScenarioGroupID,
                    Visibility = Enum.GetName(typeof(VisibilityEnum), (VisibilityEnum)k.VisibilityID)
                }).First();
        }
        
        public IEnumerable<ScenarioGroupResource> AuthorizedGroups(HttpRequestContext request)
        {
            string authString = Convert.ToString(request.RouteData.Values["authString"]);
            return _repository.Queryable().Where(k => k.Secret == authString || k.ScenarioGroupID == 1).ToList()
                .Select(k => new ScenarioGroupResource()
            {
                Name = k.Name,
                ScenarioGroupID = k.ScenarioGroupID,
                Visibility = Enum.GetName(typeof(VisibilityEnum),(VisibilityEnum)k.VisibilityID)
            });
        }

        public ScenarioGroup AddAuthenticatedScenarioGroup(ScenarioGroupResource postdata)
        {
            ScenarioGroup scenarioGroup = new ScenarioGroup()
            {
                Name = postdata.Name,
                Secret = postdata.Secret,
                OwnedBy = LcaDataModel.User.AUTHENTICATED_USER,
                VisibilityID = Convert.ToInt32(String.IsNullOrEmpty(postdata.Visibility)
                                   ? VisibilityEnum.Private
                                   : Enum.Parse(typeof(VisibilityEnum), postdata.Visibility)),
                ObjectState = ObjectState.Added
            };
            _repository.Insert(scenarioGroup);
            return scenarioGroup;
        }

        public ScenarioGroup UpdateScenarioGroup(int scenarioGroupId, ScenarioGroupResource putdata)
        {
            ScenarioGroup currentGroup = _repository.Queryable().Where(k => k.ScenarioGroupID == scenarioGroupId)
                .First();

            if (!String.IsNullOrEmpty(putdata.Secret))
                currentGroup.Secret = putdata.Secret;
            if (!String.IsNullOrEmpty(putdata.Name))
                currentGroup.Name = putdata.Name;
            if (!String.IsNullOrEmpty(putdata.Visibility))
            {
                VisibilityEnum sgid;
                if (Enum.TryParse<VisibilityEnum>(putdata.Visibility, true, out sgid))
                    currentGroup.VisibilityID = Convert.ToInt32(sgid);
            }
            currentGroup.ObjectState = ObjectState.Modified;
            _repository.Update(currentGroup);
            return currentGroup;
        }


        public IEnumerable<ScenarioGroupResource> ConfigListAllGroups()
        {
            // list all known groups via configuration controller.
            return _repository.Queryable().ToList().Select(k => new ScenarioGroupResource()
                {
                    Name = k.Name,
                    ScenarioGroupID = k.ScenarioGroupID,
                    Visibility = Enum.GetName(typeof(VisibilityEnum), (VisibilityEnum)k.VisibilityID)
                });
        }
    }
}

