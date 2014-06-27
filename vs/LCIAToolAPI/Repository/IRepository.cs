﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using System.Linq.Expressions;

namespace Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity FindById(object id);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        void Insert(TEntity entity);
        RepositoryQuery<TEntity> Query();
        IRepository<T> GetRepository<T>() where T : class;
        IQueryable<TEntity> Queryable();
    }
}
