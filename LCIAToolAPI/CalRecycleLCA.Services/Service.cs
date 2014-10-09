using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;

namespace Services
{
    public class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        #region Private Fields
        private readonly IRepository<TEntity> _repository;
        #endregion Private Fields

        #region Constructor
        public Service(IRepository<TEntity> repository) { _repository = repository; }
        #endregion Constructor


        public virtual TEntity FindById(object id)
        {
            return _repository.FindById(id);
        }

        public virtual void Insert(TEntity entity) { _repository.Insert(entity); }

        public virtual void InsertGraph(TEntity entity) { _repository.InsertGraph(entity); }

        public virtual void Update(TEntity entity) { _repository.Update(entity); }

        public virtual void Delete(object id) { _repository.Delete(id); }

        public virtual void Delete(TEntity entity) { _repository.Delete(entity); }

        public RepositoryQuery<TEntity> Query() { return _repository.Query(); }
    }
}
