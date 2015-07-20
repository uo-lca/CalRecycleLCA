using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Web;
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
        bool DetermineType(ref ParamResource p);
        ParamResource AdHocParam(string query);
        IEnumerable<ParamResource> GetParams(int scenarioId);
        IEnumerable<ParamResource> GetParamResource(IEnumerable<Param> Ps);
        IEnumerable<Param> NewOrUpdateParam(int scenarioId, ParamResource post, ref CacheTracker cacheTracker);
        IEnumerable<Param> PostNewParams(int scenarioId, ref CacheTracker cacheTracker);
        IEnumerable<Param> UpdateParam(int paramId, ParamResource post, ref CacheTracker cacheTracker);
        void DeleteParam(int paramId, ref CacheTracker cacheTracker);
    }

    public class ParamService : Service<Param>, IParamService
    {
        private IRepository<Param> _repository;
        public ParamService(IRepositoryAsync<Param> repository)
            : base(repository)
        {
            _repository = repository;
        }

        private bool isvalid(int? id)
        {
            return (id != null && id != 0);
        }

        public bool DetermineType(ref ParamResource p)
        {
            if (isvalid(p.FragmentFlowID))
                p.ParamTypeID = 1;
            //else if (isvalid(p.CompositionDataID))
            //    p.ParamTypeID = 5;
            else if (!isvalid(p.FlowID))
                return false;
            else if (isvalid(p.LCIAMethodID))
                p.ParamTypeID = 10;
            else if (isvalid(p.FlowPropertyID))
                if (_repository.GetCompositionId((int)p.FlowID, (int)p.FlowPropertyID) != null)
                    p.ParamTypeID = 5; // this is an internal indicator- ParamTypeID==5 should never appear in the wild
                else
                    p.ParamTypeID = 4;
            else if (isvalid(p.ProcessID))
                if (_repository.IsDissipation((int)p.ProcessID, (int)p.FlowID))
                    p.ParamTypeID = 6;
                else
                    p.ParamTypeID = 8;
            else
                return false;
            return true;
        }

        public ParamResource AdHocParam(string query)
        {
            ParamResource PR = new ParamResource();
            var queryFields = HttpUtility.ParseQueryString(query);
            var idFields = new List<string>() { "FragmentFlowID", "FlowID", "ProcessID", "LCIAMethodID", "FlowPropertyID", "CompositionDataID" };
            foreach (string field in idFields)
            {
                var property = PR.GetType().GetProperty(field);
                var content = queryFields.Get(field);
                if (content != null)
                    property.SetValue(PR, Convert.ToInt32(content));
            }

            if (DetermineType(ref PR))
                return PR;
            else
                return null;
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
            if (/*isvalid(post.ParamTypeID) || */DetermineType(ref post)) // ignore user-supplied ParamTypeID
                return _repository.PostParam(scenarioId, post, ref cacheTracker);
            else
                return null;
        }

        public IEnumerable<Param> PostNewParams(int scenarioId, ref CacheTracker cacheTracker)
        {
            return _repository.PostNewParams(scenarioId, ref cacheTracker);
        }

        public IEnumerable<Param> UpdateParam(int paramId, ParamResource put, ref CacheTracker cacheTracker)
        {
            if (/*isvalid(put.ParamTypeID) || */DetermineType(ref put)) // ignore user-supplied ParamTypeID
                return _repository.UpdateParam(paramId, put, ref cacheTracker);
            else 
                return null;
        }

        public void DeleteParam(int paramId, ref CacheTracker cacheTracker)
        {
            _repository.DeleteParam(paramId, ref cacheTracker);
        }
    }
}
