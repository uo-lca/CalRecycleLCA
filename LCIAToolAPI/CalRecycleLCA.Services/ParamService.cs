using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using CalRecycleLCA.Repositories;

namespace CalRecycleLCA.Services
{
    public interface IParamService : IService<Param>
    {
        IEnumerable<ParamResource> GetParams(int scenarioId);
        IEnumerable<ParamResource> GetParamResource(IEnumerable<Param> Ps);
        IEnumerable<Param> NewOrUpdateParam(int scenarioId, ParamResource post, ref CacheTracker cacheTracker);
        IEnumerable<Param> UpdateParam(int paramId, ParamResource post, ref CacheTracker cacheTracker);
        void DeleteParam(Param P, ref CacheTracker cacheTracker);
    }

    public class ParamService : Service<Param>, IParamService
    {
        private IRepositoryAsync<Param> _repository;
        public ParamService(IRepositoryAsync<Param> repository)
            : base(repository)
        {
            _repository = repository;
        }


        public IEnumerable<ParamResource> GetParams(int scenarioId)
        {
            List<Param> P = _repository.Queryable().Where(s => s.ScenarioID == scenarioId).ToList();
            List<ParamResource> paramList = new List<ParamResource>();
            foreach (Param p in P)
            {
                paramList.AddRange(_repository.GetParamResource(p));
            }
            return paramList;
        }

        public IEnumerable<ParamResource> GetParamResource(IEnumerable<Param> Ps)
        {
            List<ParamResource> paramList = new List<ParamResource>();
            foreach (Param p in Ps)
            {
                paramList.AddRange(_repository.GetParamResource(p));
            }
            return paramList;
        }
        
        /// <summary>
        /// Create or update a parameter based on its IDs.  No difference between PUT and POST, since 
        /// only one param is allowed for each matching ID pair for each scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public IEnumerable<Param> NewOrUpdateParam(int scenarioId, ParamResource post, ref CacheTracker cacheTracker)
        {
            return _repository.PostParam(scenarioId, post, ref cacheTracker);
        }

        public IEnumerable<Param> UpdateParam(int paramId, ParamResource put, ref CacheTracker cacheTracker)
        {
            return _repository.UpdateParam(paramId, put, ref cacheTracker);
        }

        public void DeleteParam(Param P, ref CacheTracker cacheTracker)
        {
            _repository.DeleteParam(P, ref cacheTracker);
        }
    }
}
