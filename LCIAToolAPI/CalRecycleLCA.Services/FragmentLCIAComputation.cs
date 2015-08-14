using Entities.Models;
using LcaDataModel;
using Ninject;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public class FragmentLCIAComputation : IFragmentLCIAComputation
    {
        [Inject]
        private readonly IFragmentTraversalV2 _fragmentTraversalV2;
        [Inject]
        private readonly ILCIAComputationV2 _lciaComputationV2;
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly INodeCacheService _nodeCacheService;
        [Inject]
        private readonly IScoreCacheService _scoreCacheService;
        [Inject]
        private readonly ILCIAMethodService _lciaMethodService;
        [Inject]
        private readonly IFragmentService _fragmentService;
        [Inject]
        private readonly IScenarioService _ScenarioService;
        [Inject]
        private readonly IProcessFlowService _processFlowService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;

        private T verifiedDependency<T>(T dependency) where T : class
        {
            if (dependency == null)
                throw new ArgumentNullException("dependency", String.Format("Type: {0}", dependency.GetType().ToString()));
            else
                return dependency;
        }

        // diagnostics
        private CounterTimer sw_local, sw_ff, sw_cache, sw_lcia, sw_traverse;
        private List<ScoreCache> currentCache;
        private bool[] fragsThisRound;

        public FragmentLCIAComputation(IFragmentTraversalV2 fragmentTraversalV2,
            ILCIAComputationV2 lciaComputationV2,
            IFragmentFlowService fragmentFlowService,
            IScoreCacheService scoreCacheService,
            INodeCacheService nodeCacheService,
            ILCIAMethodService lciaMethodService,
            IFragmentService fragmentService,
            IScenarioService scenarioService,
            IProcessFlowService processFlowService,
            IUnitOfWork unitOfWork)
        {
            _fragmentTraversalV2 = verifiedDependency(fragmentTraversalV2);
            _lciaComputationV2 = verifiedDependency(lciaComputationV2);
            _fragmentFlowService = verifiedDependency(fragmentFlowService);
            _scoreCacheService = verifiedDependency(scoreCacheService);
            _nodeCacheService = verifiedDependency(nodeCacheService);
            _lciaMethodService = verifiedDependency(lciaMethodService);
            _fragmentService = verifiedDependency(fragmentService);
            _ScenarioService = verifiedDependency(scenarioService);
            _processFlowService = verifiedDependency(processFlowService);
            _unitOfWork = verifiedDependency(unitOfWork);

            sw_local = new CounterTimer();
            sw_ff = new CounterTimer();
            sw_cache = new CounterTimer();
            sw_lcia = new CounterTimer();
            sw_traverse = new CounterTimer();

            currentCache = new List<ScoreCache>();

            fragsThisRound = new bool[_fragmentService.Count() + 1];
        }

        private void ClearLocalVars()
        {
            fragsThisRound = new bool[_fragmentService.Count() + 1];
            currentCache.Clear();
            sw_traverse.Reset();
            sw_ff.Reset();
            sw_cache.Reset();
            sw_lcia.Reset();
            sw_local.Reset();
        }

        /// <summary>
        /// Generates a list of FragmentFlowIDs that would be encountered during traversal of a given scenario, so that 
        /// cache records matching them can be cloned.
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public List<int> FragmentsEncountered(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            List<int> found = new List<int>();
            List<int> queue = new List<int>() { fragmentId };
            while (queue.Count() > 0 )
            {
                if (!found.Contains(queue[0]))
                {
                    found.Add(queue[0]);
                    queue.AddRange(SubFragmentsEncountered(queue[0], scenarioId));
                }
                queue.RemoveAt(0);
            }
            return found;
        }

        /// <summary>
        /// List of fragments encountered- called recursively
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        private List<int> SubFragmentsEncountered(int fragmentId, int scenarioId)
        {
            List<int> fs = new List<int>();
            var subfrags = _fragmentFlowService.LGetFlowsByFragment(fragmentId).Where(k => k.NodeTypeID == 2 || k.NodeTypeID == 4);
            foreach (var ff in subfrags)
            {
                var term = _fragmentFlowService.Terminate(_fragmentFlowService.GetNodeModel(ff), scenarioId, true);
                if (term.NodeTypeID == 2)
                    fs.Add((int)term.SubFragmentID);
            }
            return fs;
        }

        public List<int> ParentFragments(List<int> fragmentFlowIds, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            // for each ffid in, determine its fragment
            // for each fragment, recurse on a list of ffids that resolve to it.  check FragmentSubstitution and FragmentNodeFragment
            // then send back the whole list
            List<int> frags = _fragmentFlowService.Queryable()
                .Where(k => fragmentFlowIds.Contains(k.FragmentFlowID))
                .Select(k => k.FragmentID).ToList();

            List<int> subffs = _fragmentFlowService.ListParents(frags, scenarioId).ToList();

            if (subffs.Count() > 0)
                frags.AddRange(ParentFragments(subffs, scenarioId));

            return frags.Distinct().ToList();
        }


        public IEnumerable<NodeCache> FragmentTraverse(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            sw_traverse.Reset();
            sw_traverse.CStart();

            // this will eventually become private after diagnostics are done
            if (_nodeCacheService.IsCached(fragmentId, scenarioId))
            {
                sw_traverse.CStop();
                return _nodeCacheService.Queryable()
                    //.Where(nc => nc.FragmentFlow.FragmentID == fragmentId)
                    .Where(nc => nc.ScenarioID == scenarioId).ToList();
            }
            else
            {
                sw_traverse.Click("is not cached");
                var nodeCaches = _fragmentTraversalV2.EnterTraversal((int)fragmentId, scenarioId).Select(k => new NodeCache
                {
                    FragmentFlowID = k.FragmentFlowID,
                    ScenarioID = k.ScenarioID,
                    FlowMagnitude = k.FlowMagnitude,
                    NodeWeight = k.NodeWeight,
                    ILCDEntityID = k.ILCDEntityID,
                    ObjectState = ObjectState.Added
                });
                sw_traverse.Click("exit traverse");
                _unitOfWork.SetAutoDetectChanges(false);
                _nodeCacheService.InsertGraphRange(nodeCaches);
                _unitOfWork.SaveChanges();
                _unitOfWork.SetAutoDetectChanges(true);
                sw_traverse.Click("update cache");
                sw_traverse.CStop();
                return nodeCaches;
            }


        }

        public IEnumerable<ScoreCache> FragmentLCIAComputeNoSave(int fragmentId, int scenarioId)
        {
            ClearLocalVars();
            sw_local.CStart();

            FragmentTraverse(fragmentId, scenarioId);
            sw_local.Click("traversal");
            
            var lciaMethods = _lciaMethodService.QueryActiveMethods();
            sw_local.Click("lcia");

            var newCaches = FragmentFlowLCIA(fragmentId, scenarioId, lciaMethods);//.ToList();

            sw_local.CStop();
            sw_ff.CStop();
            sw_cache.CStop();
            sw_lcia.CStop();

            return newCaches;
            


        }

        public void FragmentLCIAComputeSave(int fragmentId, int scenarioId)
        {
            ClearLocalVars();
            sw_local.CStart();

            FragmentTraverse(fragmentId, scenarioId);
            sw_local.Click("traversal");

            var lciaMethods = _lciaMethodService.QueryActiveMethods();
            sw_local.Click("lcia");

            FragmentFlowLCIA(fragmentId, scenarioId, lciaMethods, true);//.ToList();

            sw_local.CStop();
            sw_ff.CStop();
            sw_cache.CStop();
            sw_lcia.CStop();

            _ScenarioService.UnMarkStale(scenarioId);
            _unitOfWork.SaveChanges();

            return;

        }

        private IEnumerable<FragmentLCIAModel> JoinCaches(IQueryable<FragmentFlow> FFs, int scenarioId)
        {
            // internally: return one record per FragmentFlow; aggregate in ResourceService
            return FFs.Join(_nodeCacheService.Queryable().Where(x => x.ScenarioID == scenarioId),
                        ff => ff.FragmentFlowID,
                        nc => nc.FragmentFlowID,
                        (ff, nc) => new { ff, nc })
                    .Join(_scoreCacheService.Queryable().Where(x => x.ScenarioID == scenarioId),
                        l => l.nc.FragmentFlowID,
                        sc => sc.FragmentFlowID,
                        (nc, sc) => new { nodeCaches = nc, scoreCaches = sc }).Select(s => new FragmentLCIAModel
                        {
                            FragmentFlowID = s.nodeCaches.ff.FragmentFlowID,
                            FragmentStageID = s.nodeCaches.ff.FragmentStageID,
                            LCIAMethodID = s.scoreCaches == null ? 0 : s.scoreCaches.LCIAMethodID,
                            NodeWeight = s.nodeCaches.nc.NodeWeight,
                            ImpactScore = s.scoreCaches == null ? 0 : s.scoreCaches.ImpactScore
                        });
        }

        public IEnumerable<FragmentLCIAModel> RecursiveFragmentLCIA(int fragmentId, int scenarioId)
        {
            // stack 3 things: 
            // non-subfragment nodes; 
            // subfragment non-descend nodes; 
            // results from recursion on descend nodes
            // for now: return one record per FragmentFlow
            var lcia = JoinCaches(_fragmentFlowService.Queryable()
                                    .Where(x => x.FragmentID == fragmentId)
                                    .Where(x => x.NodeTypeID != 2), scenarioId)
                                    .ToList();

            lcia.AddRange(JoinCaches(_fragmentFlowService.Queryable()
                                    .Where(x => x.FragmentID == fragmentId)
                                    .Where(x => x.NodeTypeID == 2)
                                    .Where(x => x.FragmentNodeFragments.FirstOrDefault().Descend == false), scenarioId));

            var recurse = _fragmentFlowService.Queryable()
                .Where(x => x.FragmentID == fragmentId)
                .Where(x => x.NodeTypeID == 2)
                .Where(x => x.FragmentNodeFragments.FirstOrDefault().Descend == true)
                .Join(_nodeCacheService.Queryable().Where(x => x.ScenarioID == scenarioId),
                    ff => ff.FragmentFlowID,
                    nc => nc.FragmentFlowID,
                    (ff,nc) => new {ff, nc})
                .Select(d => new NodeCacheModel
                {
                    FragmentID = d.ff.FragmentNodeFragments.FirstOrDefault().SubFragmentID,
                    NodeWeight = d.nc.NodeWeight
                }).ToList();

            foreach (var subfragment in recurse)
                lcia.AddRange(RecursiveFragmentLCIA(subfragment.FragmentID, scenarioId)
                    .Where(k => k.LCIAMethodID != 0) // do we need this? does this cause bugs?
                    .Select(k => new FragmentLCIAModel
                    {
                        FragmentFlowID = k.FragmentFlowID,
                        FragmentStageID = k.FragmentStageID,
                        LCIAMethodID = k.LCIAMethodID,
                        NodeWeight = subfragment.NodeWeight * k.NodeWeight,
                        ImpactScore = k.ImpactScore
                    }));

            return lcia;
        }


        public IEnumerable<FragmentLCIAModel> FragmentLCIA(int fragmentId, int scenarioId)
        {
            return JoinCaches(_fragmentFlowService.Queryable()
                .Where(x => x.FragmentID == fragmentId),scenarioId);
        }

        public IEnumerable<FragmentLCIAModel> Sensitivity(int fragmentId, ParamResource p)
        {
            return GetSensitivities(fragmentId, p);
        }

        public List<InventoryModel> ComputeFragmentLCI(int fragmentId, int scenarioId)
        {
            var sw = new CounterTimer();
            sw.Start();
            sw.Click("zero");

            var tFlows = FragmentFlowsVisited(fragmentId, scenarioId, 1.0);

            sw.Click("visited");

            // first handle cutoffs and IOs
            var exchanges = tFlows.Where(k => k.NodeType=="Cutoff" || k.NodeType=="InputOutput")
                .Select(k => new InventoryModel() 
                {
                    FlowID = (int)k.FlowID,
                    DirectionID = Convert.ToInt32(Enum.Parse(typeof(DirectionEnum),k.Direction)),
                    Result = k.NodeWeight,
                    StDev = 0
                }).ToList();

            sw.Click(String.Format("Added {0} cutoff/IOs",exchanges.Count));

            // then add in processes
            var procs = tFlows.Where(k => k.NodeType=="Process").GroupBy(k => k.ProcessID)
                .Select(group => new FragmentFlowResource() 
                {
                    ProcessID = group.Key,
                    NodeWeight = group.Sum(a => a.NodeWeight)
                });

            foreach (var process in procs)
            {
                exchanges.AddRange(_lciaComputationV2.ComputeProcessLCI((int)process.ProcessID,scenarioId,false)
                    .Select( k => new InventoryModel()
                    {
                        FlowID = k.FlowID,
                        DirectionID = k.DirectionID,
                        Result = k.Result * process.NodeWeight,
                        StDev = k.StDev * process.NodeWeight
                    }));
                sw.Click(String.Format("Process LCIA {0}",process.ProcessID));
            }

            sw.Stop();

            // then aggregate
            return exchanges.GroupBy(k => new { k.FlowID, k.DirectionID }).Select(group => new InventoryModel()
                {
                    FlowID = group.Key.FlowID,
                    DirectionID = group.Key.DirectionID,
                    Result = group.Sum(a => a.Result),
                    StDev = Math.Sqrt((double)group.Sum(a => (a.Result * a.Result * a.StDev * a.StDev)))
                }).ToList();
        }

        private List<FragmentFlowResource> FragmentFlowsVisited(int fragmentId, int scenarioId, double scale)
        {
            var tFlows = _fragmentFlowService.GetTerminatedFlows(fragmentId, scenarioId).ToList();

            var subFrags = tFlows.Where(k => String.Equals(k.NodeType, "Fragment")).ToList();

            foreach (var subfrag in subFrags)
            {
                tFlows.RemoveAll(k => k.FragmentFlowID == subfrag.FragmentFlowID);
                tFlows.AddRange(FragmentFlowsVisited((int)subfrag.SubFragmentID, scenarioId, (double)subfrag.NodeWeight));
            }
            foreach (var flow in tFlows)
                flow.NodeWeight *= scale;

            return tFlows;
        }


        /* *************************
         * Private Functions
         * */

        private IEnumerable<ScoreCache> FragmentFlowLCIA(int fragmentId, int scenarioId, IEnumerable<int> lciaMethods,
            bool save = false)
        {
            List<ScoreCache> scoreCaches = new List<ScoreCache>();

            if (fragsThisRound[fragmentId])
                return scoreCaches; // already done this frag this round.
            fragsThisRound[fragmentId] = true;

            // set score cache for fragment / scenario / method: iterate through
            // fragmentflows 
            currentCache.AddRange(_scoreCacheService.GetScenarioCaches(fragmentId, scenarioId).ToList());

            sw_local.Click("caches");

            var fragmentFlows = _fragmentFlowService.GetLCIAFlows(fragmentId);

            sw_local.Click("GTF");


            foreach (var item in fragmentFlows)
            {
                
                sw_ff.CStart();
                //if (item.FlowID == null)
                  //  item.FlowID = 0; // TODO: this should be set to whatever is the fragment's inflow-- ??

                // case 1: terminate FF directly -- judged FASTER 346-425 ms
                var fragmentNode = _fragmentFlowService.Terminate(item, scenarioId, true); // true => do Background 
                // case 2: terminate via GetResource -- judged SLOWER 533-582 ms
                //var fragmentNode = _fragmentFlowService.Terminate(_fragmentFlowService.GetResource(item), scenarioId, true);
                // propagate to Traversal


                sw_ff.CStop();
                
                if (fragmentNode.NodeTypeID == 2)
                {
                    //recursive LCIA computation, results to cache
                    sw_local.Click("recurse " + Convert.ToString((int)fragmentNode.SubFragmentID));
                    FragmentFlowLCIA((int)fragmentNode.SubFragmentID, scenarioId, lciaMethods, save);
                }

                scoreCaches.AddRange(GetScoreCaches(item.FragmentFlowID, fragmentNode, lciaMethods));

            }

            if (scoreCaches.Count() > 0 && save == true)
            {
                sw_cache.CStart();
                //disable this until results have been cached - doesn't seem to increase performance
                _unitOfWork.SetAutoDetectChanges(false);
                _scoreCacheService.InsertGraphRange(scoreCaches);

                _unitOfWork.SaveChanges();
                //enable this after results have been cached -  it was turned off to increase performance
                _unitOfWork.SetAutoDetectChanges(true);
                sw_cache.CStop();
            }

            return scoreCaches; 
        }



        //   public void SetScoreCache(int? targetId, int? nodeTypeId, IEnumerable<LCIAMethod> lciaMethods, int scenarioId, int fragmentFlowId)
        // {
        //   error('not implemented');
        //}
        private IEnumerable<ScoreCache> GetScoreCaches(int fragmentFlowId, FlowTerminationModel fragmentNode, IEnumerable<int> lciaMethods)
        {
            List<ScoreCache> scoreCachesInProgress = new List<ScoreCache>();

            IEnumerable<int> haveLciaMethods = currentCache.Where(x => x.FragmentFlowID == fragmentFlowId)
                                                        .Select(x => x.LCIAMethodID);

            IEnumerable<int> needLciaMethods = lciaMethods.Where(m => !haveLciaMethods.Contains(m));

            if (needLciaMethods.Count() == 0)
                return scoreCachesInProgress;
            sw_lcia.CStart();

            switch (fragmentNode.NodeTypeID)
            {
                case 1:
                    {
                        var lcias = _lciaComputationV2.ProcessLCIA((int)fragmentNode.ProcessID, needLciaMethods, fragmentNode.ScenarioID);

                        scoreCachesInProgress.AddRange(lcias.GroupBy(k => new { k.LCIAMethodID, k.ScenarioID })
                            .Select(group => new ScoreCache()
                            {
                                ScenarioID = group.Key.ScenarioID,
                                FragmentFlowID = fragmentFlowId,
                                LCIAMethodID = group.Key.LCIAMethodID,
                                ImpactScore = group.Sum(a => a.Result),
                                ObjectState = ObjectState.Added
                            }));
                    }
                    /*
                    foreach (var lciaMethodId in needLciaMethods.AsQueryable())
                    {
                        if (scores.Any(s => s.LCIAMethodID == lciaMethodId))
                        {
                            double score = scores
                                .Where(x => x.LCIAMethodID == lciaMethodId)
                                .Select(x => Convert.ToDouble(x.Total)).First();

                            scoreCachesInProgress.Add(new ScoreCache()
                            {
                                ScenarioID = fragmentNode.ScenarioID,
                                FragmentFlowID = fragmentFlowId,
                                LCIAMethodID = lciaMethodId,
                                ImpactScore = score,
                                ObjectState = ObjectState.Added
                            });

                        }
                    }
                     * */
                    break;
                case 2:
                    {
                        scoreCachesInProgress.AddRange(FragmentLCIA((int)fragmentNode.SubFragmentID, fragmentNode.ScenarioID)
                            .Where(r => needLciaMethods.Contains(r.LCIAMethodID)).GroupBy(r => r.LCIAMethodID)
                            .Select(group => new ScoreCache()
                            {
                                ScenarioID = fragmentNode.ScenarioID,
                                FragmentFlowID = fragmentFlowId,
                                LCIAMethodID = group.Key,
                                ImpactScore = Convert.ToDouble(group.Sum(a => a.Result)),
                                ObjectState = ObjectState.Added
                            }));
                    }
                    break;
            }  /* end of switch NodeType */

            sw_lcia.CStop(String.Format("FFID {0} Type {1}", fragmentFlowId, fragmentNode.NodeTypeID));

            return scoreCachesInProgress;
        } /* end of SetScoreCache */

        /// <summary>
        /// Returns a list of impact score deltas associated with a unit parameter value for a given fragment.
        /// At each node, score equals nodeCache * scoreCache or nc * sc; sensitivity is nc * dsc/dx + dnc/dx * sc; 
        /// for each param type, either dnc/dx or dsc/dx is zero.
        /// for type 1- sc stays constant, dx = dependency weight; dnc/dx = parent node weight<br/>
        /// for type 8- nc stays constant, dx = emission qty; dsc/dx = LCIA factor<br/>
        /// for type 10- nc stays constant, dx = LCIA factor; dsc/dx = emission qty<br/>
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private List<FragmentLCIAModel> GetSensitivities(int fragmentId, ParamResource p)
        {
            List<FragmentLCIAModel> results = new List<FragmentLCIAModel>();

            sw_local.Click(fragmentId.ToString() + " sensitivities");
            var ff = _fragmentFlowService.GetTerminatedFlows(fragmentId, p.ScenarioID).ToList();
            sw_local.Click(fragmentId.ToString() + " terminated");

            switch (p.ParamTypeID)
            {
                case 1:
                    {
                        var sensflow = ff.Where(k => k.FragmentFlowID == p.FragmentFlowID).FirstOrDefault();
                        if (sensflow != null)
                        {
                            // sensitivity parameter is in current fragment
                            double scale = (double)ff.Where(k => k.FragmentFlowID == sensflow.ParentFragmentFlowID)
                                .Select(k => k.NodeWeight).First();
                            if (scale != 0)
                            {
                                if (sensflow.NodeWeight == 0)
                                {
                                    // sensitivity path was not traversed
                                    var sens_ff = _fragmentTraversalV2.SensitivityTraverse(sensflow, p.ScenarioID);
                                    // sens_nc is a list of minimally-populated FragmentFlowResources, starting with unity 
                                    // at the test FragmentFlowID
                                    foreach (var node in sens_ff)
                                    {
                                        results.AddRange(LookupScoreCaches(node, scale, p.ScenarioID));
                                        ff.RemoveAll(k => k.FragmentFlowID == node.FragmentFlowID);
                                    }
                                }
                                else
                                {
                                    // path was already traversed-- all we need to do is re-scale it
                                    var sens_ff = DescendentFlows(ff, sensflow.FragmentFlowID);
                                    // need to re-compute flow scaling, unfortch.
                                    // a little documentation: 
                                    // parent node weight * dependency * flow_conv / flow_exch = child node weight
                                    // GetNodeScaling returns node_conv = flow_conv / flow_exch
                                    // so PNW * dependency * node_conv = CNW
                                    // we want to replace dependency with 1
                                    // so we want PNW * node_conv = New_CNW
                                    // Thus, the scaling to apply to each descendent node is New_CNW / CNW
                                    // so that after scaling, it will have New_CNW
                                    // so we want scale to equal PNW * node_conv / CNW
                                    // this is equal to the inverse of the current dependency value
                                    double node_conv = _fragmentFlowService.GetNodeScaling(sensflow, p.ScenarioID);
                                    scale = scale * node_conv / (double)sensflow.NodeWeight;

                                    foreach (var node in sens_ff)
                                    {
                                        results.AddRange(LookupScoreCaches(node, scale, p.ScenarioID));
                                        ff.RemoveAll(k => k.FragmentFlowID == node.FragmentFlowID);
                                    }
                                }
                            }
                            else
                            {
                                // parent node is 0, so sensitivity is 0
                                // here- nothing to do. simply don't add any results.
                            }
                            
                        }
                        break;
                    }
                case 4:
                case 5:
                case 6:
                    {
                        throw new NotImplementedException("Param Type not implemented.");
                        //break;
                    }
                case 8:
                    {
                        foreach (var node in ff.Where(k => k.NodeType == "Process")
                            .Where(k => k.ProcessID == p.ProcessID).ToList())
                        {
                            results.AddRange(LookupFlowCFs(node, (int)p.FlowID, p.ScenarioID));
                            ff.RemoveAll(k => k.FragmentFlowID == node.FragmentFlowID);
                        }

                        break;
                    }
                case 10:
                    {
                        foreach (var node in ff.Where(k => k.NodeType == "Process").ToList())
                        {
                            results.AddRange(LookupFlowQuantities(node, (int)p.FlowID, (int)p.LCIAMethodID, p.ScenarioID));
                            ff.RemoveAll(k => k.FragmentFlowID == node.FragmentFlowID);
                        }
                        break;
                    }
            }

            foreach (var fragmentflow in ff.Where(k => k.NodeType == "Fragment").ToList())
            {
                // recurse on sub-fragments
                results.AddRange(AggregateFragmentSensitivity((int)fragmentflow.SubFragmentID, p)
                    .Select(s => new FragmentLCIAModel()
                    {
                        FragmentFlowID = fragmentflow.FragmentFlowID,
                        FragmentStageID = fragmentflow.FragmentStageID,
                        LCIAMethodID = s.LCIAMethodID,
                        NodeWeight = (double)fragmentflow.NodeWeight,
                        ImpactScore = s.ImpactScore
                        //Result = 0.0
                    }));
            }
            //foreach (var elem in results)
            //    elem.Result = (double)(elem.NodeWeight * elem.ImpactScore);

            return results;



        }

        /// <summary>
        /// Given a FragmentFlow Resource, enumerate all the cached scores
        /// </summary>
        /// <param name="nc"></param>
        /// <param name="scale"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        private List<FragmentLCIAModel> LookupScoreCaches(FragmentFlowResource nc, double scale, int scenarioId)
        {
            return _scoreCacheService.GetFragmentFlowCaches(nc.FragmentFlowID, scenarioId)
                .Select(k => new FragmentLCIAModel()
                {
                    FragmentFlowID = nc.FragmentFlowID,
                    FragmentStageID = nc.FragmentStageID,
                    LCIAMethodID = k.LCIAMethodID,
                    NodeWeight = (double)nc.NodeWeight * scale,
                    ImpactScore = k.ImpactScore
//                    Result = 0.0
                }).ToList();
        }

        /// <summary>
        /// Recursive function to return a subset of fragmentflows which are descendents of a given fragmentflowID
        /// </summary>
        /// <param name="ff"></param>
        /// <param name="fragmentFlowId"></param>
        /// <returns></returns>
        private List<FragmentFlowResource> DescendentFlows(List<FragmentFlowResource> ff, int fragmentFlowId)
        {
            List<FragmentFlowResource> descendents = ff.Where(k => k.FragmentFlowID == fragmentFlowId).ToList();
            var children = ff.Where(k => k.ParentFragmentFlowID == fragmentFlowId);
            foreach (var child in children)
                descendents.AddRange(DescendentFlows(ff, child.FragmentFlowID));
            return descendents;
        }

        /// <summary>
        /// Returns a process's sensitivity to a given flow.  The process-flow pairing must exist in ProcessFlow, 
        /// or else no results are returned.
        /// 
        /// Basically, this is a lookup of flows across LCIA methods
        /// </summary>
        /// <param name="nc"></param>
        /// <returns></returns>
        private List<FragmentLCIAModel> LookupFlowCFs(FragmentFlowResource nc, int flowId, int scenarioId)
        {
            return _processFlowService.GetEmissionSensitivity((int)nc.ProcessID, flowId, scenarioId)
                .Select(k => new FragmentLCIAModel()
                {
                    FragmentFlowID = nc.FragmentFlowID,
                    FragmentStageID = nc.FragmentStageID,
                    LCIAMethodID = k.LCIAMethodID,
                    NodeWeight = (double)nc.NodeWeight,
                    ImpactScore = k.Factor
                }).ToList();
        }

        /// <summary>
        /// Returns a process's sensitivity to a given LCIA factor, defined by a flowId.
        /// 
        /// Basically, this is a lookup of a Processflow record.
        /// </summary>
        /// <param name="nc"></param>
        /// <param name="flowId"></param>
        /// <param name="lciaMethodId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        private List<FragmentLCIAModel> LookupFlowQuantities(FragmentFlowResource nc, int flowId, int lciaMethodId, int scenarioId)
        {
            return _processFlowService.GetEmissions((int)nc.ProcessID, scenarioId)
                .Where(k => k.FlowID == flowId)
                .Select(k => new FragmentLCIAModel() 
                {
                    FragmentFlowID = nc.FragmentFlowID,
                    FragmentStageID = nc.FragmentStageID,
                    LCIAMethodID = lciaMethodId,
                    NodeWeight = (double)nc.NodeWeight,
                    ImpactScore = (double)k.Result
                }).ToList();
        }

        /// <summary>
        /// Perform sensitivity analysis for subfragment, aggregate SubFragmentFlow results into 
        /// FragmentFlow (unit) ImapctScore
        /// </summary>
        /// <param name="fragmentId"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private List<FragmentLCIAModel> AggregateFragmentSensitivity(int fragmentId, ParamResource p)
        {
            return GetSensitivities(fragmentId, p)
                .GroupBy(s => s.LCIAMethodID)
                .Select(g => new FragmentLCIAModel()
                {
                    LCIAMethodID = g.Key,
                    ImpactScore = g.Select(x => x.Result).Sum()
//                    Result = 0.0
                }).ToList();
        }
    }
}
