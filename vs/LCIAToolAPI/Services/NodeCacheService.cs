using Data;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class NodeCacheService : Service<NodeCache>, INodeCacheService
    {
        [Inject]
        private readonly IRepository<NodeCache> _repository;


        public NodeCacheService(IRepository<NodeCache> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
