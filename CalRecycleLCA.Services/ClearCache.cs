using LcaDataModel;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Pattern;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace CalRecycleLCA.Services
{
    public class ClearCache : IClearCache
    {
        [Inject]
        private readonly IScoreCacheService _scoreCacheService;
        [Inject]
        private readonly INodeCacheService _nodeCacheService;
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;

        public ClearCache(
           INodeCacheService nodeCacheService,
           IScoreCacheService scoreCacheService,
           IFragmentFlowService fragmentFlowService,
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
            //needs to be updated to take into account the removal of nodecacheid as a foreign key in the scorecache
            //var scoreCaches = _scoreCacheService.Queryable()
            //    .Join(_nodeCacheService.Queryable(), sc => sc.NodeCacheID, nc => nc.NodeCacheID, (sc, nc) => new { sc, nc })
            //    .Join(_fragmentFlowService.Queryable(), nc => nc.nc.FragmentFlowID, ff => ff.FragmentFlowID, (nc, ff) => new { nc, ff })
            //    .Where(x => x.ff.FragmentID == fragmentId)
            //    .Where(x => x.nc.nc.ScenarioID == scenarioId).ToList()
            //    .Select(x => x.nc.sc)
            //    .ToList();

            //scoreCaches.ForEach(x => 
            //    {
            //        x.ObjectState = ObjectState.Deleted;
            //        _scoreCacheService.Delete(x.ScoreCacheID); 
            //    });

            //_unitOfWork.SaveChanges();
        }
    }
}
