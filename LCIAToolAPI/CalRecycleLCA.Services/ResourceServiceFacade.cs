using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Entities.Models;
using Ninject;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using System.Runtime.CompilerServices;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace CalRecycleLCA.Services
{
    /// <summary>
    /// Web API Services Facade
    /// Gets LcaDataModel entities and transforms them to web API resources.
    /// Executes Traversal and Computation as needed.
    /// </summary>
    public class ResourceServiceFacade : IResourceServiceFacade
    {

        private CounterTimer sw_lcia;

        #region Dependency Injection
        //
        // LcaDataModel services
        //
        [Inject]
        private readonly IClassificationService _ClassificationService;
        [Inject]
        private readonly IFragmentService _FragmentService;
        [Inject]
        private readonly IFragmentFlowService _FragmentFlowService;
        [Inject]
        private readonly IFragmentStageService _FragmentStageService;
        [Inject]
        private readonly IFlowService _FlowService;
        [Inject]
        private readonly IFlowPropertyService _FlowPropertyService;
        [Inject]
        private readonly IFlowTypeService _FlowTypeService;
        [Inject]
        private readonly IImpactCategoryService _ImpactCategoryService;
        [Inject]
        private readonly ILCIAMethodService _LciaMethodService;
        [Inject]
        private readonly IProcessService _ProcessService;
        [Inject]
        private readonly IProcessFlowService _ProcessFlowService;
        [Inject]
        private readonly IScenarioService _ScenarioService;
        [Inject]
        private readonly IParamService _ParamService;
        [Inject]
        private readonly IFlowFlowPropertyService _FlowFlowPropertyService;
        [Inject]
        private readonly ILCIAService _LCIAService;
        //
        // Traversal and Computation components
        //
        [Inject]
        private readonly IFragmentLCIAComputation _FragmentLCIAComputation;
        [Inject]
        private readonly ILCIAComputationV2 _LCIAComputation;
        [Inject]
        private readonly ICacheManager _CacheManager;

        private T verifiedDependency<T>(T dependency) where T : class
        {
            if (dependency == null)
            {
                throw new ArgumentNullException("dependency", String.Format("Type: {0}", dependency.GetType().ToString()));
            }
            else
            {
                return dependency;
            }
        }

        /// <summary>
        /// Constructor for use with Ninject dependency injection
        /// </summary>
        public ResourceServiceFacade(
                               IClassificationService classificationService,
                               IFragmentService fragmentService,
                               IFragmentFlowService fragmentFlowService,
                               IFragmentStageService fragmentStageService,
                               IFragmentLCIAComputation fragmentLCIAComputation,
                               IFlowService flowService,
                               IFlowPropertyService flowPropertyService,
                               IFlowTypeService flowTypeService,
                               IImpactCategoryService impactCategoryService,
                               ILCIAComputationV2 lciaComputation,
                               ILCIAMethodService lciaMethodService,
                               IProcessService processService,
                               IProcessFlowService processFlowService,
                               IScenarioService scenarioService,
                               IParamService paramService,
                               IFlowFlowPropertyService flowFlowPropertyService,
                               ILCIAService lciaService,
                               ICacheManager cacheManager) 
        {
            _ClassificationService = verifiedDependency(classificationService);
            _FragmentService = verifiedDependency(fragmentService);
            _FragmentFlowService = verifiedDependency(fragmentFlowService);
            _FragmentStageService = verifiedDependency(fragmentStageService);
            _FragmentLCIAComputation = verifiedDependency(fragmentLCIAComputation);
            _FlowService = verifiedDependency(flowService);
            _FlowPropertyService = verifiedDependency(flowPropertyService);
            _FlowTypeService = verifiedDependency(flowTypeService);
            _ImpactCategoryService = verifiedDependency(impactCategoryService);
            _LCIAComputation = verifiedDependency(lciaComputation);
            _LciaMethodService = verifiedDependency(lciaMethodService);
            _ProcessService = verifiedDependency(processService);
            _ProcessFlowService = verifiedDependency(processFlowService);
            _ScenarioService = verifiedDependency(scenarioService);
            _ParamService = verifiedDependency(paramService);
            _FlowFlowPropertyService = verifiedDependency(flowFlowPropertyService);
            _LCIAService = verifiedDependency(lciaService);
            _CacheManager = verifiedDependency(cacheManager);

            sw_lcia = new CounterTimer();
        }
        #endregion

        #region Model-Resource Transforms

        // Transformation methods

        /// <summary>
        /// Transform an LCIAMethod object to a LCIAMethodResource object
        /// </summary>
        /// <param name="lm"></param>
        /// <returns></returns>
        public LCIAMethodResource Transform(LCIAMethod lm)
        {
            return new LCIAMethodResource
            {
                LCIAMethodID = lm.LCIAMethodID,
                Name = lm.Name,
                Methodology = lm.Methodology,
                ImpactCategoryID = lm.ImpactCategoryID, 
                ImpactIndicator = lm.ImpactIndicator,
                ReferenceYear = lm.ReferenceYear,
                Duration = lm.Duration,
                ImpactLocation = lm.ImpactLocation,
                IndicatorType = lm.IndicatorType.Name,
                Normalization = lm.Normalization, 
                Weighting = lm.Weighting, 
                UseAdvice = lm.UseAdvice,
                ReferenceFlowPropertyID = lm.ReferenceFlowPropertyID,
                ReferenceFlowProperty = _FlowPropertyService.GetResource(lm.ReferenceFlowPropertyID),
                UUID = lm.ILCDEntity.UUID,
                Version = lm.ILCDEntity.Version
            };
        }

        public FragmentResource Transform(Fragment f)
        {
            var term = _FragmentFlowService.GetInFlow(f.FragmentID);
            return new FragmentResource
            {
                FragmentID = f.FragmentID,
                Name = f.Name,
                ReferenceFragmentFlowID = _FragmentFlowService.GetReferenceFlowID(f.FragmentID),
                TermFlowID = term.FlowID,
                Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)term.DirectionID),
                UUID = f.ILCDEntity.UUID,
                Version = f.ILCDEntity.Version
            };
        }

        public DetailedLCIAResource Transform(LCIAModel m)
        {
            return new DetailedLCIAResource
            {
                //LCIAMethodID = TransformNullable(m.LCIAMethodID, "LCIAModel.LCIAMethodID"),
                FlowID = m.FlowID, 
                Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)m.DirectionID),
                Content = m.Composition,
                Dissipation = m.Dissipation,
                Quantity = Convert.ToDouble(m.Quantity),
                Factor = Convert.ToDouble(m.Factor),
                Result = Convert.ToDouble(m.Result)
            };
        }

        /*
        public AggregateLCIAResource Transform(FragmentLCIAModel m)
        {
            //ICollection<DetailedLCIAResource> details = new List<DetailedLCIAResource>();
            //if (m.NodeLCIAResults.Count() != 0)
            //    details = m.NodeLCIAResults.Select(k => Transform(k)).ToList();

            return new AggregateLCIAResource
            {
                FragmentStageID = TransformNullable(m.FragmentStageID, "FragmentLCIAModel.FragmentFlowID"),
                CumulativeResult = Convert.ToDouble(m.Result),
                LCIADetail = new List<DetailedLCIAResource>()
                //LCIADetail = details
            };
        }
        */
        public LCIAResultResource Transform(LCIAResult m, int processId)
        {
            return new LCIAResultResource
            {
                LCIAMethodID = m.LCIAMethodID,
                ScenarioID = (int)m.ScenarioID,
                LCIAScore = new List<AggregateLCIAResource>
                { new AggregateLCIAResource 
                    {
                        ProcessID = processId,
                        CumulativeResult = m.Total,
                        LCIADetail = new List<DetailedLCIAResource>()
                    }
                }
            };
        }
        #endregion

        #region Service Providers
        // Get list methods 

        /// <summary>
        /// Get LCIAMethodResource list with optional filter by ImpactCategory
        /// </summary>
        public IEnumerable<LCIAMethodResource> GetActiveLCIAMethodResources(int? impactCategoryID = null)
        {
            IEnumerable<LCIAMethod> lciaMethods;
            var query = _LciaMethodService.FetchActiveMethods()
                                                .ToList();
            
            if (impactCategoryID == null)
            {
                lciaMethods = ((IEnumerable)query).Cast<LCIAMethod>();
            }
            else
            {
                lciaMethods = ((IEnumerable)query).Cast<LCIAMethod>().Where(d => d.ImpactCategoryID == impactCategoryID);
            }
            return lciaMethods.Select(lm => Transform(lm)).ToList();
        }

        /// <summary>
        /// Get factors for a given LCIA method. Only return paired (i.e. non-null Flow) factors.
        /// This could maybe be scenario-sensitive, as Characterization Params would change factors
        /// on a scenario-specific basis.
        /// </summary>
        /// <param name="lciaMethodId"></param>
        /// <returns></returns>
        public IEnumerable<LCIAFactorResource> GetLCIAFactors(int lciaMethodId)
        {
            return _LCIAService.QueryFactors(lciaMethodId);
        }

        /// <summary>
        /// Execute fragment traversal and return computation results in FragmentFlowResource objects
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <param name="scenarioID">ScenarioID filter for NodeCache</param>
        /// <returns>List of FragmentFlowResource objects</returns>
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = Scenario.MODEL_BASE_CASE_ID) {
            var ffs = _FragmentFlowService.GetTerminatedFlows(fragmentID, scenarioID)
                .ToList();

            foreach (var ff in ffs)
                ff.FlowPropertyMagnitudes = ff.FlowPropertyMagnitudes.Select(k => new FlowPropertyMagnitude()
                {
                    FlowPropertyID = k.FlowPropertyID,
                    Unit = _FlowPropertyService.Queryable().Where(fp => fp.FlowPropertyID == k.FlowPropertyID)
                        .Select(fp => fp.UnitGroup.ReferenceUnit).FirstOrDefault(),
                    Magnitude = k.Magnitude
                }).ToList();

            List<int> balanceFlows = _FragmentFlowService.ListBalanceFlows(fragmentID).Intersect(ffs.Select(k => k.FragmentFlowID)).ToList();

            foreach (var bal in balanceFlows)
            {
                var balff = ffs.Where(k => k.FragmentFlowID==bal);
                int balanceNode = balff.Select(k => (int)k.ParentFragmentFlowID).First();
                var refFp = balff.Select(k => k.FlowPropertyMagnitudes.First()).Select(k => k.FlowPropertyID).First();
    
                foreach (var ff in ffs)
                {
                    if (ff.FragmentFlowID == balanceNode && ff.FlowPropertyMagnitudes.Select(k => k.FlowPropertyID == refFp).Count()>0)
                        ff.isConserved = true;
                    else if (ff.ParentFragmentFlowID == balanceNode && ff.FlowPropertyMagnitudes.Select(k => k.FlowPropertyID == refFp).Count()>0)
                        ff.isConserved = true;

                    if (ff.FragmentFlowID == bal)
                        ff.isBalanceFlow = true;
                }
            }
            return ffs;
        }

        /// <summary>
        /// Get list of all flows.  Optional flowtypeID is 1 = Intermediate, 2 = Elementary, 0 = both
        /// </summary>
        /// <param name="flowtypeID">from FlowType</param>
        /// <param name="relID">ID of related fragment or process</param>
        /// <returns>List of FlowResource objects</returns>
        public IEnumerable<FlowResource> GetFlows(int flowtypeID)
        {
            return _FlowService.GetFlows(flowtypeID);
        }

        public IEnumerable<FlowResource> GetCompositionFlows()
        {
            return _FlowService.GetCompositionFlows();
        }


        /// <summary>
        /// Get a single flow. should really do bounds-checking
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public IEnumerable<FlowResource> GetFlow(int flowId)
        {
            return new List<FlowResource>() {_FlowService.GetFlow(flowId) };
        }

        public IEnumerable<FlowResource> GetFlowsByLCIAMethod(int lciaMethodId)
        {
            return _FlowService.GetFlowsByLCIAMethod(lciaMethodId);
        }

        /// <summary>
        /// Get list of flows related to a fragment (via FragmentFlow)
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowResource objects</returns>
        public IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID)
        {
            return _FlowService.GetFlowsByFragment(fragmentID);
        }

        public IEnumerable<FragmentStageResource> GetStagesByFragment(int fragmentID)
        {
            if (fragmentID == 0)
                return _FragmentStageService.Queryable()
                    .Select(k => new FragmentStageResource() 
                    { 
                        FragmentStageID = k.FragmentStageID,
                        Name = k.Name
                    }).ToList();
            else
                return _FragmentFlowService.GetFragmentStages(fragmentID);
        }

        public IEnumerable<FragmentStageResource> GetRecursiveStagesByFragment(int fragmentId)
        {
            return _FragmentFlowService.GetRecursiveFragmentStages(fragmentId);
        }


        /// <summary>
        /// Get list of processflow resources.  
        /// 
        /// This needs to be privacy aware, since the processflows are what are being protected. 
        /// Several conceivable levels of privacy: 
        ///  * list nothing (the current behavior); 
        ///  * list only flow names with no quantities; 
        ///  * list only elementary flows;
        ///  * (list only intermediate flows-- these are fragmentflows and so are already listed)
        ///    [ Important to qualify-- fragment traversal (without quantity)- 
        ///      so traversal results must be protected too. effectively this means you can't 
        ///      descend into certain fragments]
        /// Ultimately, could be switch IsPrivate rather than bool.
        /// </summary>
        public IEnumerable<ProcessFlowResource> GetProcessFlows(int processID, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {

            if (_ProcessService.IsPrivate(processID))
                return new List<ProcessFlowResource>();
            else
                return _LCIAComputation.ComputeProcessLCI(processID, scenarioId).Select(k => new ProcessFlowResource()
                {
                    Flow = _FlowService.GetFlow(k.FlowID),
                    Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)k.DirectionID),
                    // VarName = omitted,
                    Content = k.Composition,
                    Dissipation = k.Dissipation,
                    Quantity = k.Result == null 
                        ? (double)k.Composition * (double)k.Dissipation
                        : (double)k.Result,
                    STDev = k.StDev == null ? 0.0 : (double)k.StDev
                }).ToList();
;
        }

        public IEnumerable<ProcessDissipationResource> GetProcessDissipation(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _ProcessService.GetDissipation(processId, scenarioId);
        }


        /// <summary>
        /// Return all flowproperties
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FlowPropertyResource> GetFlowProperties()
        {
            return _FlowPropertyService.GetAllResources();
        }

        public IEnumerable<FlowPropertyMagnitude> GetFlowFlowProperties(int flowId)
        {
            return _FlowFlowPropertyService.GetFlowPropertyMagnitudes(flowId)
                .Select(k => new FlowPropertyMagnitude()
                {
                    FlowProperty = _FlowPropertyService.GetResource((int)k.FlowPropertyID),
                    Magnitude = k.Magnitude
                });
        }

        /// <summary>
        /// Get list of flow properties related to a process (via FlowFlowProperty and ProcessFlow)
        /// </summary>
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID)
        {
            return _FlowPropertyService.GetFlowPropertiesByProcess(processID);
        }

        /// <summary>
        /// Get FlowProperty data related to fragment and transform to API resource model
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowPropertyResource objects</returns>
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID)
        {
            return _FlowPropertyService.GetFlowPropertiesByFragment(fragmentID);
        }

         /// <summary>
        /// Get Fragment data and transform to API resource model
        /// </summary>
        /// <returns>List of FragmentResource objects</returns>
        public IEnumerable<FragmentResource> GetFragmentResources(int? fragmentId = null)
        {
            if (fragmentId == null)
                return _FragmentService.Query()
                    .Include(k => k.ILCDEntity)
                    .Select().Select(f => Transform(f)).ToList();
            else
                return _FragmentService.Query(f => f.FragmentID == fragmentId)
                    .Include(k => k.ILCDEntity)
                    .Select().Select(f => Transform(f)).ToList();
        }

        /// <summary>
        /// Get Process data and transform to API resource model
        /// </summary>
        /// <param name="flowTypeID">Optional process flow type filter</param>
        /// <returns>List of ProcessResource objects</returns>
        public IEnumerable<ProcessResource> GetProcesses(int flowTypeID = 0)
        {
            return _ProcessService.GetProcesses(flowTypeID);
        }

        /// <summary>
        /// Single process
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public IEnumerable<ProcessResource> GetProcess(int processId)
        {
            return _ProcessService.GetProcess(processId);
        }

        /// <summary>
        /// Get ImpactCategory data and transform to API resource model
        /// Omit categories that are not related to any LCIAMethod
        /// </summary>
        /// <returns>List of ImpactCategoryResource objects</returns>
        public IEnumerable<ImpactCategoryResource> GetImpactCategories()
        {
            IEnumerable<ImpactCategory> data = _ImpactCategoryService.Queryable()
                .Where(i => i.LCIAMethods.Count() > 0);
            return data.Select(d => new ImpactCategoryResource
            {
                ImpactCategoryID = d.ID,
                Name = d.Name
            }).ToList();
        }

        /// <summary>
        /// Execute Process LCIA and return computation results in LCIAResultResource objects
        /// Work around problem in LCIA computation: should be filtering out LCIA with Geography 
        /// </summary>
        /// <returns>LCIAResultResource or null if lciaMethodID not found</returns> 
        public LCIAResultResource GetProcessLCIAResult(int processId, int lciaMethodId, int scenarioId = Scenario.MODEL_BASE_CASE_ID) {
            var lciaMethod = new List<int> { lciaMethodId };
                LCIAResult lciaResult = _LCIAComputation.ProcessLCIA(processId, lciaMethod, scenarioId).First();
                var lciaAgg = new AggregateLCIAResource
                    {
                        ProcessID = processId,
                        CumulativeResult = (double)lciaResult.Total,
                        LCIADetail = (_ProcessService.IsPrivate(processId)
                            ? new List<DetailedLCIAResource>()
                            : lciaResult.LCIADetail.Select(m => Transform(m)).ToList())
                    };  
                var lciaResource = new LCIAResultResource
                {
                    LCIAMethodID = lciaMethodId,
                    ScenarioID = scenarioId,
                    LCIAScore = new List<AggregateLCIAResource>() { lciaAgg }
                };
                return lciaResource;
        }

        
        public IEnumerable<LCIAResultResource> GetProcessLCIAResults(int processId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            IEnumerable<LCIAResult> lciaResults = _LCIAComputation.LCIACompute(processId, scenarioId);
            return lciaResults.Select(m => Transform(m, processId));
        }
        

        private IEnumerable<LCIAResultResource> GroupFragmentLCIA(List<FragmentLCIAModel> ff_results, int scenarioId)
        {
            return ff_results
                .GroupBy(r => r.LCIAMethodID)
                .Select(group => new LCIAResultResource
                {
                    ScenarioID = scenarioId,
                    LCIAMethodID = group.Key,
                    LCIAScore = group.GroupBy(k => k.FragmentStageID)
                        .Select(grp => new AggregateLCIAResource
                        {
                            FragmentStageID = grp.Key,
                            CumulativeResult = grp.Sum(a => a.Result),
                            LCIADetail = new List<DetailedLCIAResource>()
                        }).ToList()
                });
        }

        /// <summary>
        /// Execute Fragment LCIA and return computation result as FragmentLCIAResource object
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="lciaMethodID"></param>
        /// <param name="scenarioID">Defaults to base scenario</param>
        /// <returns>Fragment LCIA results for given parameters</returns> 
        public LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID) {
            sw_lcia.CStart();
            var result = GroupFragmentLCIA(_FragmentLCIAComputation
                    .FragmentLCIA(fragmentID, scenarioID, lciaMethodID).ToList(),
                    scenarioID)
                .Where(r => r.LCIAMethodID == lciaMethodID).FirstOrDefault();
            sw_lcia.CStop("Flat FID " + Convert.ToString(fragmentID) + " LCIA " + Convert.ToString(lciaMethodID));
            return result;

        }

        public LCIAResultResource GetRecursiveFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID)
        {
            sw_lcia.CStart();
            var result = GroupFragmentLCIA(_FragmentLCIAComputation
                    .RecursiveFragmentLCIA(fragmentID, scenarioID, lciaMethodID).ToList(),
                    scenarioID)
                .Where(r => r.LCIAMethodID == lciaMethodID).FirstOrDefault();
            sw_lcia.CStop("Reur FID " + Convert.ToString(fragmentID) + " LCIA " + Convert.ToString(lciaMethodID));
            return result;
        }


        /// <summary>
        /// Execute Fragment LCIA and return computation results in FragmentLCIAResource objects-- one lcia method, all scenarios
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="lciaMethodID"></param>
        /// <param name="scenarioGroupID">Scenario group of the user making request</param>
        /// <returns>List of LCIAResultResource objects</returns> 
        public IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID, int scenarioGroupID = 1)
        {
            IEnumerable<Scenario> scenarios = _ScenarioService.Queryable().Where(s => s.ScenarioGroupID == scenarioGroupID).ToList();
            return scenarios.Select(s => GetFragmentLCIAResults(fragmentID, lciaMethodID, s.ScenarioID)).ToList();
        }

        public IEnumerable<LCIAResultResource> GetRecursiveFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID, int scenarioGroupID = 1)
        {
            sw_lcia.Reset();

            IEnumerable<Scenario> scenarios = _ScenarioService.Queryable().Where(s => s.ScenarioGroupID == scenarioGroupID).ToList();
            return scenarios.Select(s => GetRecursiveFragmentLCIAResults(fragmentID, lciaMethodID, s.ScenarioID)).ToList();
        }
        
        /// <summary>
        /// Execute Fragment LCIA and return computation results in FragmentLCIAResource objects-- one scenario, all LCIA methods
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="lciaMethodID"></param>
        /// <param name="scenarioGroupID">Scenario group of the user making request</param>
        /// <returns>List of LCIAResultResource objects</returns> 
        public IEnumerable<LCIAResultResource> GetFragmentLCIAResultsAllMethods(int fragmentID, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            IEnumerable<int> lciaMethods = _LciaMethodService.QueryActiveMethods();
            return lciaMethods.Select(s => GetFragmentLCIAResults(fragmentID, s, scenarioId)).ToList();
        }

        public IEnumerable<LCIAResultResource> GetRecursiveFragmentLCIAResultsAllMethods(int fragmentID, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            sw_lcia.Reset();

            IEnumerable<int> lciaMethods = _LciaMethodService.QueryActiveMethods();
            return lciaMethods.Select(s => GetRecursiveFragmentLCIAResults(fragmentID, s, scenarioId)).ToList();
        }
        
        public IEnumerable<LCIAResultResource> GetFragmentSensitivity(int fragmentId, int paramId)
        {
            List<Param> Ps = _ParamService.Queryable()
                .Where(k => k.ParamID == paramId)
                .ToList();
            return GetFragmentSensitivity(fragmentId, _ParamService.GetParamResource(Ps).First());
        }

        public IEnumerable<LCIAResultResource> GetFragmentSensitivity(int fragmentId, ParamResource p)
        {
            var results = _FragmentLCIAComputation.Sensitivity(fragmentId, p);
            return results.GroupBy(r => r.LCIAMethodID)
                .Select(group => new LCIAResultResource()
                {
                    ScenarioID = p.ScenarioID,
                    LCIAMethodID = group.Key,
                    LCIAScore = group.GroupBy(k => 
                        k.FragmentStageID // uncomment below for diagnostics
//                        new 
//                    { 
//                        k.FragmentFlowID,
//                        k.FragmentStageID
//                    }
                    )
                        .Select(grp => new AggregateLCIAResource
                        {
//                            FragmentFlowID = grp.Key.FragmentFlowID,
//                            FragmentStageID = grp.Key.FragmentStageID,
                            FragmentStageID = grp.Key,
                            CumulativeResult = grp.Sum(a => a.Result),
                            LCIADetail = new List<DetailedLCIAResource>()
                        }).ToList()
                });
        }

        public IEnumerable<ProcessFlowResource> GetFragmentLCI(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _FragmentLCIAComputation.ComputeFragmentLCI(fragmentId, scenarioId)
                .Select(k => new ProcessFlowResource()
                {
                    Flow = _FlowService.GetFlow(k.FlowID),
                    Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)k.DirectionID),
                    Quantity = (double)k.Result,
                    STDev = (double)k.StDev
                }).OrderBy(k => k.Flow.FlowID);
        }

        /// <summary>
        /// Get scenario types, with optional ScenarioGroup argument to constrain the ScenarioIDs returned
        /// </summary>
        /// <returns>List of ScenarioResource objects</returns>
        public IEnumerable<ScenarioResource> GetScenarios()
        {
            IEnumerable<Scenario> scenarios = _ScenarioService.Queryable();
            return scenarios.Select(c => _ScenarioService.GetResource(c)).ToList();
        }

        public IEnumerable<ScenarioResource> GetScenarios(int? userGroupID)
        {
            IEnumerable<Scenario> scenarios = _ScenarioService.Queryable()
                .Where(c => (c.ScenarioGroupID == 1 || c.ScenarioGroupID == userGroupID));
            return scenarios.Select(c => _ScenarioService.GetResource(c)).ToList();
        }

        /// <summary>
        /// Get FlowType data and transform to API resource model
        /// </summary>
        /// <returns>List of FlowTypeResource objects</returns>
        public IEnumerable<FlowTypeResource> GetFlowTypes()
        {
            IEnumerable<FlowType> data = _FlowTypeService.Queryable();
            return data.Select(d => new FlowTypeResource
            {
                FlowTypeID = d.ID,
                Name = d.Name
            }).ToList();
        }

        private ScenarioResource CheckTopLevelFragment(ScenarioResource scenario, int cloneScenario)
        {
            if (_FragmentService.Query(k => k.FragmentID == scenario.TopLevelFragmentID) == null)
                scenario.TopLevelFragmentID = _ScenarioService.Queryable()
                    .Where(k => k.ScenarioID == cloneScenario)
                    .Select(k => k.TopLevelFragmentID).First();

            var term = _FragmentFlowService.GetInFlow(scenario.TopLevelFragmentID);
            if (scenario.ReferenceFlowID == 0)
            {
                scenario.ReferenceFlowID = term.FlowID;
            }
            else
            {
                // alternative ReferenceFlowID is allowed only if it can be converted to the same units as term.FlowID
                var conv = _FlowFlowPropertyService.FlowConv(scenario.ReferenceFlowID, term.FlowID);
                if (conv == 0 || conv == null)
                    return null; // must be commensurable
                // else - nothing required
            }
            scenario.ReferenceDirection = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)term.DirectionID);
            return scenario;
        }

        public int AddScenario(ScenarioResource postScenario, int scenarioGroupId, int cloneScenario)
        {
            // request has already been authorized-- all we need to do is fill in the pieces
            // ScenarioResource has fields:
            //0  ScenarioID
            //X  ScenarioGroupID
            //R  TopLevelFragmentID
            //?  ActivityLevel
            //?  Name 
            //?  ReferenceFlowID
            //A  ReferenceDirectionID
            //A  ScenarioGroupID 
            // of these, ScenarioID is ignored; TopLevelFragmentID is required; ActivityLevel is optional; Name is optional.
            // ScenarioGroupID is set; Reference flow + direction are set; ScenarioID is db-generated.
            postScenario = CheckTopLevelFragment(postScenario, cloneScenario);
            if (postScenario == null)
                return -1; // -1 should generate a 400 or 415 http error

            if (postScenario.Name == null)
                postScenario.Name = "User-Generated Scenario";
            if (postScenario.ActivityLevel == 0)
                postScenario.ActivityLevel = 1;
            postScenario.ScenarioGroupID = scenarioGroupId;
            return _CacheManager.CreateScenario(postScenario, cloneScenario);
        }

        public bool UpdateScenario(int scenarioId, ScenarioResource putScenario)
        {
            CacheTracker cacheTracker = new CacheTracker();
            Scenario scenario;
            if (putScenario.TopLevelFragmentID == 0 && putScenario.ReferenceFlowID == 0)
                // just update details
                scenario = _ScenarioService.UpdateScenarioDetails(scenarioId, putScenario);
            else
            {
                if (putScenario.TopLevelFragmentID == 0)
                    putScenario.TopLevelFragmentID = _ScenarioService.Query(k => k.ScenarioID == scenarioId)
                        .Select(k => k.TopLevelFragmentID).First();
                putScenario = CheckTopLevelFragment(putScenario, scenarioId);
                if (putScenario == null)
                    return false;
                scenario = _ScenarioService.UpdateScenarioFlow(scenarioId, putScenario, ref cacheTracker);
            }
            if (scenario != null)
            {
                return _CacheManager.ImplementScenarioChanges(scenario.ScenarioID, cacheTracker);
            }
            return false;
        }

        public void DeleteScenario(int scenarioId)
        {
            _CacheManager.DeleteScenario(scenarioId);
        }

        public bool DeleteParam(int scenarioId, int paramId)
        {
            CacheTracker cacheTracker = new CacheTracker();
            int p_scenarioId = _ParamService.Query(k => k.ParamID == paramId)
                .Select(k => k.ScenarioID).First();
            if (p_scenarioId == scenarioId)
            {
                _ParamService.DeleteParam(paramId, ref cacheTracker);
                return _CacheManager.ImplementScenarioChanges(scenarioId, cacheTracker);
            }
            return false;
        }

        /// <summary>
        /// Retrieves a list of params belonging to a given scenario.
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public IEnumerable<ParamResource> GetParams(int scenarioId)
        {
            return _ParamService.GetParams(scenarioId);
        }

        public IEnumerable<ParamResource> AddParam(int scenarioId, ParamResource postParam)
        {
            CacheTracker cacheTracker = new CacheTracker();
            List<Param> Ps = _ParamService.NewOrUpdateParam(scenarioId, postParam, ref cacheTracker).ToList();
            Ps.AddRange(_ParamService.PostNewParams(scenarioId, ref cacheTracker));
            if (_CacheManager.ImplementScenarioChanges(scenarioId, cacheTracker))
                return _ParamService.GetParamResource(Ps);
            else
                return null;
        }

        public IEnumerable<ParamResource> UpdateParam(int scenarioId, int paramId, ParamResource putParam)
        {
            CacheTracker cacheTracker = new CacheTracker();
            IEnumerable<Param> Ps = new List<Param>();
            if (_ParamService.Query(k => k.ParamID == paramId).Select(k => k.ScenarioID).First() == scenarioId)
                Ps = _ParamService.UpdateParam(paramId, putParam, ref cacheTracker);
            else
                Ps = _ParamService.NewOrUpdateParam(scenarioId, putParam, ref cacheTracker);
            if (_CacheManager.ImplementScenarioChanges(scenarioId, cacheTracker))
                return _ParamService.GetParamResource(Ps);
            else
                return null;
        }

        public IEnumerable<ParamResource> UpdateParams(int scenarioId, IEnumerable<ParamResource> putParams)
        {
            CacheTracker cacheTracker = new CacheTracker();
            List<Param> Ps = new List<Param>();
            List<int> existingParamIds = _ParamService.Queryable()
                .Where(p => p.ScenarioID == scenarioId)
                .Select(p => p.ParamID).ToList();

            foreach (ParamResource put in putParams)
                Ps.AddRange(_ParamService.NewOrUpdateParam(scenarioId, put, ref cacheTracker));

            Ps.AddRange(_ParamService.PostNewParams(scenarioId, ref cacheTracker));

            // determine omitted parameters
            List<int> omittedParamIds = existingParamIds.AsQueryable()
                .Where(pid => !cacheTracker.ParamModified.Contains(pid))
                .Where(pid => !cacheTracker.ParamUnchanged.Contains(pid))
                .ToList();

            foreach (int del in omittedParamIds)
                _ParamService.DeleteParam(del, ref cacheTracker);

            if (_CacheManager.ImplementScenarioChanges(scenarioId, cacheTracker))
                return _ParamService.GetParams(scenarioId);
            else
                return null;
        }

      #endregion  
    }
}
