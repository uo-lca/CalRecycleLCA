﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;


namespace Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal IDbContext Context;
        internal IDbSet<TEntity> DbSet;
        internal IUnitOfWork UnitOfWork;

        public Repository(IDbContext context, IUnitOfWork unitOfWork)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
            UnitOfWork = unitOfWork;

        }

        public virtual TEntity FindById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual void InsertGraph(TEntity entity)
        {
           
            DbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Attach(entity);
        }

        public virtual void Delete(object id)
        {

            var entity = DbSet.Find(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
            ((IObjectState)entity).ObjectState = ObjectState.Deleted;
        }

        public virtual void Insert(TEntity entity)
        {
            
            DbSet.Attach(entity);
            
        }

        public virtual RepositoryQuery<TEntity> Query()
        {
            var repositoryGetFluentHelper =
                new RepositoryQuery<TEntity>(this);

            return repositoryGetFluentHelper;
        }

        internal IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>>
                includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }

        public IRepository<T> GetRepository<T>() where T : class { return UnitOfWork.Repository<T>(); }

        public IQueryable<TEntity> Queryable() { return DbSet; }
    }
}
