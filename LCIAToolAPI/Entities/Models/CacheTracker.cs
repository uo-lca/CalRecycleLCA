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
            _fragmentFlows = new List<int>();
            _fragmentTraverse = new List<int>();
            ParamModified = new List<int>();
            ParamUnchanged = new List<int>();
            ParamsToPost = new List<ParamResource>();
        }
        private bool _recompute;
        private List<int> _fragmentFlows; // list of flows whose scores need to be updates
        private List<int> _fragmentTraverse; // list of flows whose fragments need to be traversed (plus parents)

        public bool NodeCacheStale { get; set; } // supersedes Fragment traverse list
        public bool ScoreCacheStale { get; set; } // supersedes FragmentFlows list
        public List<int> LCIAMethodsStale { get; set; }

        public List<ParamResource> ParamsToPost { get; set; }

        public bool Recompute { 
            get {return ( _recompute | NodeCacheStale | ScoreCacheStale
                | (LCIAMethodsStale.Count() > 0 )
                | (_fragmentFlows.Count() > 0)
                | (_fragmentTraverse.Count() > 0)); }
            set { _recompute = value; }
        }

        public List<int> FragmentFlowsStale
        {
            get { return _fragmentFlows; }
            set { _fragmentFlows = value.Distinct().ToList(); }
        }
        public List<int> ParamModified;
        public List<int> ParamUnchanged;
        public List<int> FragmentFlowsTraverse
        {
            get { return _fragmentTraverse; }
            set { _fragmentTraverse = value.Distinct().ToList(); }
        }
    }
}
