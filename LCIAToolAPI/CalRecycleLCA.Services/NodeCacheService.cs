﻿using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface INodeCacheService : IService<NodeCache>
    {
    }

    public class NodeCacheService : Service<NodeCache>, INodeCacheService
    {
        public NodeCacheService(IRepositoryAsync<NodeCache> repository)
            : base(repository)
        {
                    
        }
    }
}