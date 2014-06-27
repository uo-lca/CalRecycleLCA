﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Repository;

namespace Repository
{
    public interface IQueryFluent<TEntity> where TEntity : class
    {
        IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        IQueryFluent<TEntity> Include(Expression<Func<TEntity, object>> expression);
        IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount);
        IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector = null);
        IEnumerable<TEntity> Select();
    }
}