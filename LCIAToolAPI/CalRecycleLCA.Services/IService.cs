using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repository;

namespace Services
{
    public interface IService<TEntity> where TEntity : class
    {
        TEntity FindById(object id);
        void Insert(TEntity entity);
        void InsertGraph(TEntity entity);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        RepositoryQuery<TEntity> Query();
        
    }
}
