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
    }
}
