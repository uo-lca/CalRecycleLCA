using LcaDataModel;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ClearCache : IClearCache
    {
        [Inject]
        private readonly IService<ScoreCache> _scoreCacheService;
        [Inject]
        private readonly IService<NodeCache> _nodeCacheService;
        [Inject]
        private readonly IService<FragmentFlow> _fragmentFlowService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;

        public ClearCache(
           IService<NodeCache> nodeCacheService,
           IService<ScoreCache> scoreCacheService,
           IService<FragmentFlow> fragmentFlowService,
           IUnitOfWork unitOfWork)
        {
            if (nodeCacheService == null)
            {
                throw new ArgumentNullException("nodeCacheService is null");
            }

            _nodeCacheService = nodeCacheService;

            if (scoreCacheService == null)
            {
                throw new ArgumentNullException("scoreCacheService is null");
            }
            _scoreCacheService = scoreCacheService;

            if (fragmentFlowService == null)
            {
                throw new ArgumentNullException("fragmentFlowService is null");
            }
            _fragmentFlowService = fragmentFlowService;

            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork is null");
            }
            _unitOfWork = unitOfWork;
        }

        public void Clear(int fragmentId, int scenarioId)
        {
            var scoreCaches = _scoreCacheService.Query().Get()
                .Join(_nodeCacheService.Query().Get(), sc => sc.NodeCacheID, nc => nc.NodeCacheID, (sc, nc) => new { sc, nc })
                .Join(_fragmentFlowService.Query().Get(), nc => nc.nc.FragmentFlowID, ff => ff.FragmentFlowID, (nc, ff) => new { nc, ff })
                .Where(x => x.ff.FragmentID == fragmentId)
                .Where(x => x.nc.nc.ScenarioID == scenarioId).ToList()
                .Select(x => x.nc.sc)
                .ToList();

            scoreCaches.ForEach(x => 
                {
                    x.ObjectState = ObjectState.Deleted;
                    _scoreCacheService.Delete(x.ScoreCacheID); 
                });

            _unitOfWork.Save();
        }
    }
}
