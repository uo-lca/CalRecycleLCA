using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;

        private bool _disposed;
        private Hashtable _repositories;

        public UnitOfWork(IDbContext context)
        {
            _context = context;
        }

        //public UnitOfWork()
        //{
        //    _context = new UsedOilLCAContext();
        //}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();

            _disposed = true;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            //if (!_repositories.ContainsKey(type))
            //{
                //var repositoryType = typeof(Repository<>);

                //var repositoryInstance =
                //    Activator.CreateInstance(repositoryType
                //            .MakeGenericType(typeof(T)), _context);

                //_repositories.Add(type, repositoryInstance);

                


                if (_repositories.ContainsKey(type))
                {
                    return (IRepository<T>)_repositories[type];
                }

                var repositoryType = typeof(Repository<>);

                _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context, this));
                return (IRepository<T>)_repositories[type];
            }

            //return (IRepository<T>)_repositories[type];
        
    }
}
