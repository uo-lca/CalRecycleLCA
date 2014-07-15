using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LCIATool.Models.Repository;

namespace LCIATool.Models.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private LCAToolDevEntities1 context = new LCAToolDevEntities1();
        private GenericRepository<Fragment> fragmentRepository;
        private GenericRepository<Flow> flowRepository;

        public GenericRepository<Fragment> FragmentRepository
        {
            get
            {
                if (this.fragmentRepository == null)
                    this.fragmentRepository = new GenericRepository<Fragment>(context);
                return fragmentRepository;
            }
        }

        public GenericRepository<Flow> FlowRepository
        {
            get
            {
                if (this.flowRepository == null)
                    this.flowRepository = new GenericRepository<Flow>(context);
                return flowRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}