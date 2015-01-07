using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    /*
     * still don't really understand what to use interfaces for
    public interface ICacheTracker
    {
        public bool NodeCacheStale;
        public bool ScoreCacheStale;
        public List<int> LCIAMethodsStale;

    }
     * */
    public class CacheTracker
    {
        public CacheTracker()
        {
            LCIAMethodsStale = new List<int>();
        }
        private bool _recompute;
        public bool NodeCacheStale { get; set; }
        public bool ScoreCacheStale { get; set; }
        public List<int> LCIAMethodsStale { get; set; }

        public bool Recompute { 
            get {return ( _recompute | NodeCacheStale | ScoreCacheStale | (LCIAMethodsStale.Count() > 0 )); }
            set { _recompute = value; }
        }
    }
}
