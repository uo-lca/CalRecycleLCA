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
        private readonly INodeCacheService _NodeCacheService;
        [Inject]
        private readonly IScoreCacheService _ScoreCacheService;
        [Inject]
        private readonly IParamService _ParamService;
        [Inject]
        private readonly IFlowFlowPropertyService _FlowFlowPropertyService;
        [Inject]
        private readonly ILCIAService _LCIAService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;
        //
        // Traversal and Computation components
        //
        [Inject]
        private readonly IFragmentLCIAComputation _FragmentLCIAComputation;
        [Inject]
        private readonly ILCIAComputationV2 _LCIAComputation;

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
                               INodeCacheService nodeCacheService,
                               IScoreCacheService scoreCacheService,
                               IParamService paramService,
                               IFlowFlowPropertyService flowFlowPropertyService,
                               ILCIAService lciaService,
                               IUnitOfWork unitOfWork) 
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
            _NodeCacheService = verifiedDependency(nodeCacheService);
            _ScoreCacheService = verifiedDependency(scoreCacheService);
            _ParamService = verifiedDependency(paramService);
            _FlowFlowPropertyService = verifiedDependency(flowFlowPropertyService);
            _LCIAService = verifiedDependency(lciaService);
            _unitOfWork = verifiedDependency(unitOfWork);
        }
        #endregion

        #region Model-Resource Transforms
        // TransformNullable methods are a workaround for imprecise relationship modeling
        // in the Database EF model, LcaDataModel.
        // TODO: Define cardinality of all relationships in class diagrams, implement changes in 
        //  LcaDataModel, apply schema changes to database, and remove this workaround.

        /// <summary>
        /// Transforms int? to int 
        /// Use to handle EF entity property with type int? that should never actually be NULL.
        /// Throws exception if property has NULL value
        /// </summary>
        /// <param name="propVal">EF property value</param>
        /// <param name="propName">EF property name</param>
        /// <returns>int value</returns>
        private int TransformNullable(int? propVal, string propName)
        {
            if (propVal == null)
            {
                throw new ArgumentNullException(propName);
            }
            else
            {
                return Convert.ToInt32(propVal);
            }
        }

        /// <summary>
        /// Transforms bool? to bool 
        /// Use to handle EF entity property with type bool? that should never actually be NULL.
        /// Throws exception if property has NULL value
        /// </summary>
        /// <param name="propVal">EF property value</param>
        /// <param name="propName">EF property name</param>
        /// <returns>bool value</returns>
        private bool TransformNullable(bool? propVal, string propName)
        {
            if (propVal == null)
            {
                throw new ArgumentNullException(propName);
            }
            else
            {
                return Convert.ToBoolean(propVal);
            }
        }

        /// <summary>
        /// Transforms double? to double 
        /// Use to handle EF entity property with type double? that should never actually be NULL.
        /// Throws exception if property has NULL value
        /// </summary>
        /// <param name="propVal">EF property value</param>
        /// <param name="propName">EF property name</param>
        /// <returns>bool value</returns>
        private double TransformNullable(double? propVal, string propName)
        {
            if (propVal == null)
            {
                throw new ArgumentNullException(propName);
            }
            else
            {
                return Convert.ToDouble(propVal);
            }
        }

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
                Normalization = TransformNullable(lm.Normalization, "LCIAMethod.Normalization"),
                Weighting = TransformNullable(lm.Weighting, "LCIAMethod.Weighting"),
                UseAdvice = lm.UseAdvice,
                ReferenceFlowPropertyID = lm.ReferenceFlowPropertyID,
                ReferenceFlowProperty = _FlowPropertyService.GetResource(lm.ReferenceFlowPropertyID),
                UUID = lm.ILCDEntity.UUID,
                Version = lm.ILCDEntity.Version
            };
        }
        /*******************************
        private ICollection<FlowPropertyMagnitude> GetFlowPropertyMagnitudes(FragmentFlow ff, int scenarioID)
        {
            // MOVE TO REPO
            IEnumerable<FlowFlowProperty> ffpData = ff.Flow.FlowFlowProperties;
            IEnumerable<NodeCache> ncData = ff.NodeCaches;

            NodeCache nodeCache = ncData.Where(nc => nc.ScenarioID == scenarioID).FirstOrDefault();
            if (nodeCache == null)
            {
                return null;
            }
            else
            {
                double flowMagnitude = Convert.ToDouble(nodeCache.FlowMagnitude);
                return ffpData.Select(ffp =>
                        new FlowPropertyMagnitude
                        {
                            FlowPropertyID = Convert.ToInt32(ffp.FlowPropertyID),
                            Magnitude = flowMagnitude * Convert.ToDouble(ffp.MeanValue)
                        }).ToList();
            }

        }

        public FragmentFlowResource Transform(FragmentFlow ff, int scenarioID)
        {
            // NEED FIX
            int? nullID = null;
            return new FragmentFlowResource
            {
                FragmentFlowID = ff.FragmentFlowID,
                FragmentStageID = ff.FragmentStageID,
                Name = ff.Name,
                ShortName = ff.ShortName,
                NodeTypeID = TransformNullable(ff.NodeTypeID, "FragmentFlow.NodeTypeID"),
                DirectionID = TransformNullable(ff.DirectionID, "FragmentFlow.DirectionID"),
                FlowID = ff.FlowID,
                NodeWeight = ff.NodeCaches.Where(nc => nc.ScenarioID == scenarioID).First().NodeWeight,
                ParentFragmentFlowID = ff.ParentFragmentFlowID,
                ProcessID = (ff.NodeTypeID == 1) ? ff.FragmentNodeProcesses.FirstOrDefault().ProcessID : nullID,
                SubFragmentID = (ff.NodeTypeID == 2) ? ff.FragmentNodeFragments.FirstOrDefault().SubFragmentID : nullID,
                FlowPropertyMagnitudes = (ff.FlowID == null) ? null : GetFlowPropertyMagnitudes(ff, scenarioID)
            };
        }

         * ****************/
        /// <summary>
        /// Transform Flow to FlowResource by joining with Category data
        /// </summary>
        /// <param name="f">Instance of Flow</param>
        /// <returns>Instance of FlowResource</returns>
        public FlowResource Transform(Flow f)
        {

            int? maxHL;
            string categoryName;

            //IEnumerable<Classification> classes = _ClassificationService.Query()
            //    .Include(c => c.Category)
            //    .Filter(c => c.ILCDEntityID == f.ILCDEntityID);


            IEnumerable<Classification> classes = _ClassificationService
                                                .Query(c => c.ILCDEntityID == f.ILCDEntityID)
                                                .Include(c => c.Category)
                                                .Select();
            

            maxHL = classes.Max(c => c.Category.HierarchyLevel);
            categoryName = classes.Where(c => c.Category.HierarchyLevel == maxHL).Single().Category.Name;

            return new FlowResource
            {
                FlowID = f.FlowID,
                Name = f.Name,
                FlowTypeID = f.FlowTypeID,
                ReferenceFlowPropertyID = TransformNullable(f.ReferenceFlowProperty, "Flow.ReferenceFlowProperty"),
                CASNumber = f.CASNumber,
                Category = categoryName
            };
        }

        public FragmentStageResource Transform(FragmentStage s)
        {
            return new FragmentStageResource
            {
                FragmentStageID = s.FragmentStageID,
                FragmentID = s.FragmentID,
                Name = s.Name
            };
        }

        /*
        public FlowPropertyResource Transform(FlowProperty fp)
        {
            string unitName = "";
            if (fp.UnitGroup != null && fp.UnitGroup.UnitConversion != null)
            {
                unitName = fp.UnitGroup.UnitConversion.Unit;
            }
            return new FlowPropertyResource
            {
                FlowPropertyID = fp.FlowPropertyID,
                Name = fp.Name,
                ReferenceUnit = unitName
            };
        }
        */
        public FragmentResource Transform(Fragment f)
        {
            var term = _FragmentFlowService.GetInFlow(f.FragmentID);
            return new FragmentResource
            {
                FragmentID = f.FragmentID,
                Name = f.Name,
                ReferenceFragmentFlowID = _FragmentFlowService.GetReferenceFlowID(f.FragmentID),
                TermFlowID = term.FlowID,
                Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)term.DirectionID)
            };
        }

        public ProcessResource Transform(Process p, IList<int> emisProcesses)
        {
            return new ProcessResource
            {
                ProcessID = p.ProcessID,
                Name = p.Name,
                Geography = p.Geography,
                //ProcessTypeID = TransformNullable(p.ProcessTypeID, "Process.ProcessTypeID"),
                ReferenceTypeID = p.ReferenceTypeID,
                ReferenceFlowID = p.ReferenceFlowID,
                ReferenceYear = p.ReferenceYear,
                Version = p.ILCDEntity.Version,
                hasElementaryFlows = emisProcesses.Contains(p.ProcessID)
            };
        }

        public ProcessFlowResource Transform(ProcessFlow pf)
        {
            return new ProcessFlowResource
            {
                // ProcessFlowID = pf.ProcessFlowID,
                Flow = Transform(pf.Flow),
                Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)pf.DirectionID),
                VarName = pf.VarName,
                Magnitude = pf.Magnitude, 
                Result = pf.Result, 
                STDev = TransformNullable(pf.STDev, "ProcessFlow.STDev")
            };
        }

        public DetailedLCIAResource Transform(LCIAModel m)
        {
            return new DetailedLCIAResource
            {
                //LCIAMethodID = TransformNullable(m.LCIAMethodID, "LCIAModel.LCIAMethodID"),
                FlowID = TransformNullable(m.FlowID, "LCIAModel.FlowID"),
                Direction = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)m.DirectionID),
                Quantity = Convert.ToDouble(m.Quantity),
                Factor = Convert.ToDouble(m.Factor),
                Result = Convert.ToDouble(m.Result)
            };
        }

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


        public ScenarioResource Transform(Scenario s)
        {
            return new ScenarioResource
            {
                ScenarioID = s.ScenarioID,
                ScenarioGroupID = s.ScenarioGroupID,
                Name = s.Name,
                TopLevelFragmentID = s.TopLevelFragmentID,
                ActivityLevel = Convert.ToDouble(s.ActivityLevel),
                ReferenceFlowID = s.FlowID,
                ReferenceDirection = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)s.DirectionID)
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
            /// NEED FIX--> terminate nodes in repository layer; eager-fetch only scenario NodeCaches
            /// see http://stackoverflow.com/questions/19386501/linq-to-entities-include-where-method
            _FragmentLCIAComputation.FragmentTraverse(fragmentID, scenarioID);
            var test = _FragmentFlowService.GetTerminatedFlows(fragmentID, scenarioID)
                .ToList();
            //foreach (var ff in test)
            //    ff.FlowPropertyMagnitudes = _FlowFlowPropertyService.GetFlowPropertyMagnitudes(ff, scenarioID);

            //var fragmentFlows = _FragmentFlowService.Query(q => q.FragmentID == fragmentID)
            //                                    .Include(x => x.FragmentNodeFragments)
             //                                   .Include(x => x.FragmentNodeProcesses)
              //                                  .Include(x => x.NodeCaches)
               //                                 .Include(x => x.Flow.FlowFlowProperties)
                //                                .Select().Where(x => x.NodeCaches.Count > 0).ToList();
            //var stopgap = fragmentFlows.Where(f => f.NodeCaches.Any(nc => nc.ScenarioID == scenarioID))
              //  .Select(ff => Transform(ff, scenarioID)).ToList();
            return test;
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

        /// <summary>
        /// Get a single flow. should really do bounds-checking
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public IEnumerable<FlowResource> GetFlow(int flowId)
        {
            return _FlowService.GetFlow(flowId);
        }

        /// <summary>
        /// Get list of flows related to a fragment (via FragmentFlow)
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowResource objects</returns>
        public IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID)
        {
            return _FlowService.GetFlowsByFragment(fragmentID);
            //return GetFlows(typeof(FragmentFlow), fragmentID);
/*            return _FlowService.Query(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentID))
                .Select()
                .Select(f => Transform(f)).ToList();*/
            // return flowQuery.Select(f => Transform(f)).ToList();
        }

        public IEnumerable<FragmentStageResource> GetStagesByFragment(int fragmentID)
        {
            if (fragmentID == 0)
                return _FragmentStageService.Query()
                    .Select()
                    .Select(s => Transform(s)).ToList();
            else
                return _FragmentStageService.Query(s => s.FragmentID == fragmentID)
                    .Select()
                    .Select(s => Transform(s)).ToList();
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
        public IEnumerable<ProcessFlowResource> GetProcessFlows(int processID)
        {

            if (_ProcessService.IsPrivate(processID))
            {
                return new List<ProcessFlowResource>();
            }
            else
            {
                return _ProcessFlowService.Query(x => x.ProcessID == processID)
                                                .Include(x => x.Flow)
                                                .Select()
                                                .Select(pf => Transform(pf)).ToList();
            }
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
            return _FlowFlowPropertyService.GetFlowPropertyMagnitudes(flowId);
        }

        /// <summary>
        /// Get list of flow properties related to a process (via FlowFlowProperty and ProcessFlow)
        /// </summary>
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID)
        {
            return _FlowPropertyService.GetFlowPropertiesByProcess(processID);
            //IEnumerable<FlowProperty> flowProperties = _FlowPropertyService.Query()
            //    .Include(fp => fp.UnitGroup.UnitConversion)
            //    .Filter(fp => fp.Flows.Any(f => f.ProcessFlows.Any(pf => pf.ProcessID == processID)))
            //    .Get();
            /*var pf_flowproperties = _ProcessFlowService.Query(pf => pf.ProcessID == processID)
                .Include(pf => pf.Flow.FlowFlowProperties)
                .GroupBy(x => x.Flow.FlowFlowProperties.Select(f => f.FlowPropertyID))
                .Select(x => x.Flow.FlowFlowProperties.Select(f => f.FlowPropertyID));
            var flowProperties = _FlowPropertyService
                .Query(fp => fp.Flows.Any(f => f.ProcessFlows.Any(pf => pf.ProcessID == processID)))
                .Include(fp => fp.UnitGroup.UnitConversion)
                //.Include(fp => fp.Flows.Select(p => p.ProcessFlows)) 
                .Select().ToList();
            return flowProperties.Select(fp => Transform(fp)).ToList();
              */
        }

        /// <summary>
        /// Get FlowProperty data related to fragment and transform to API resource model
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowPropertyResource objects</returns>
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID)
        {
            return _FlowPropertyService.GetFlowPropertiesByFragment(fragmentID);
/*            var flowProperties = _FlowPropertyService
                .Query(fp => fp.Flows.Any(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentID)))
                .Include(fp => fp.UnitGroup.UnitConversion)
                //.Include(fp => fp.Flows.Select(f => f.FragmentFlows))
                .Select().ToList();
            return flowProperties.Select(fp => Transform(fp)).ToList(); */
        }

         /// <summary>
        /// Get Fragment data and transform to API resource model
        /// </summary>
        /// <returns>List of FragmentResource objects</returns>
        public IEnumerable<FragmentResource> GetFragmentResources()
        {
            IEnumerable<Fragment> fragments = _FragmentService.Queryable();
            return fragments.Select(f => Transform(f)).ToList();
        }

        /// <summary>
        /// Get a Fragment  and transform to API resource model
        /// </summary>
        /// <param name="fragmentID">FragmentID</param>
        /// <returns>FragmentResource</returns>
        public FragmentResource GetFragmentResource(int fragmentID)
        {
            Fragment fragment = _FragmentService.Find(fragmentID);
            if (fragment == null)
            {
                // error handling deferred to controller
                return null;
            }
            else
            {
                return Transform(fragment);
            }
        }

        /// <summary>
        /// Get Process data and transform to API resource model
        /// </summary>
        /// <param name="flowTypeID">Optional process flow type filter</param>
        /// <returns>List of ProcessResource objects</returns>
        public IEnumerable<ProcessResource> GetProcesses(int flowTypeID = 0)
        {
            return _ProcessService.GetProcesses(flowTypeID);
/*            var emisProcesses = _ProcessService.Query(p => p.ProcessFlows.Any(pf => pf.Flow.FlowTypeID == 2))
                .Select(x => x.ProcessID).ToList();
            
            IEnumerable<ProcessResource> pData = _ProcessService.Query()
                .Include(x => x.ILCDEntity)
                    //.Include(x => x.ProcessFlows.Select(p => p.Flow))
                .Select()
                .Select(p => Transform(p, emisProcesses));

            if (flowTypeID == 2)
                return pData.Where(p => p.hasElementaryFlows).ToList();
            else
                return pData.ToList();
 * */
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
                //IEnumerable<InventoryModel> inventory = _LCIAComputation.ComputeProcessLCI(processID, scenarioID);
                LCIAResult lciaResult = _LCIAComputation.ProcessLCIA(processId, lciaMethod, scenarioId).First();
                // var privacy_flag = _ProcessService.Query(p => p.ProcessID == processID)
                //     .Include(p => p.ILCDEntity.DataSource)
                //     .Select(p => p.ILCDEntity.DataSource.VisibilityID).First() == 2;
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
        
        /// <summary>
        /// Execute Fragment LCIA and return computation result as FragmentLCIAResource object
        /// </summary>
        /// <param name="fragmentID"></param>
        /// <param name="lciaMethodID"></param>
        /// <param name="scenarioID">Defaults to base scenario</param>
        /// <returns>Fragment LCIA results for given parameters</returns> 
        public LCIAResultResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID) {
            // check to see if cache has been populated for each scenario
            /* disable this-- cache operations should not be API driven. Bug #101 is reopened.
            if (_ScoreCacheService.Queryable().Where(s => s.FragmentFlow.FragmentID == fragmentID)
                .Where(s => s.ScenarioID == scenarioID)
                .Where(s => s.LCIAMethodID == lciaMethodID).ToList().Count() == 0)
                _FragmentLCIAComputation.FragmentLCIACompute(fragmentID, scenarioID);
             * */  
            /*
            IEnumerable<FragmentLCIAModel> aggResults = 
            * */
            return _FragmentLCIAComputation.FragmentLCIA(fragmentID, scenarioID, lciaMethodID).ToList()
                .GroupBy(r => r.LCIAMethodID)
                .Select(group => new LCIAResultResource 
                { 
                    ScenarioID = scenarioID,
                    LCIAMethodID = group.Key,
                    LCIAScore = group.GroupBy(k => k.FragmentStageID)
                        .Select(grp => new AggregateLCIAResource
                        { 
                            FragmentStageID = grp.Key,
                            CumulativeResult = grp.Sum(a => a.Result),
                            LCIADetail = new List<DetailedLCIAResource>()
                        }).ToList()

                }).Where(r => r.LCIAMethodID == lciaMethodID).First();

            /*
                
                
                .GroupBy(r => new
                {
                    r.FragmentStageID,
                    r.LCIAMethodID
                })
                .Select(group => new FragmentLCIAModel
                {
                    FragmentStageID = group.Key.FragmentStageID,
                    LCIAMethodID = group.Key.LCIAMethodID,
                    Result = group.Sum(a => a.Result)
                });
            LCIAResultResource lciaResult = new LCIAResultResource
            {
                ScenarioID = scenarioID,
                LCIAMethodID = lciaMethodID,
                LCIAScore = aggResults.Select(r => Transform(r)).ToList()
            };
            return lciaResult;
             * */
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

        public IEnumerable<LCIAResultResource> GetFragmentSensitivity(int fragmentId, int paramId)
        {
            List<Param> Ps = _ParamService.Queryable()
                .Where(k => k.ParamID == paramId)
                .ToList();
            ParamResource p = _ParamService.GetParamResource(Ps).First();
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

        /// <summary>
        /// Get scenario types, with optional ScenarioGroup argument to constrain the ScenarioIDs returned
        /// </summary>
        /// <returns>List of ScenarioResource objects</returns>
        public IEnumerable<ScenarioResource> GetScenarios()
        {
            IEnumerable<Scenario> scenarios = _ScenarioService.Queryable();
            return scenarios.Select(c => Transform(c)).ToList();
        }

        public IEnumerable<ScenarioResource> GetScenarios(int? userGroupID)
        {
            IEnumerable<Scenario> scenarios = _ScenarioService.Queryable()
                .Where(c => (c.ScenarioGroupID == 1 || c.ScenarioGroupID == userGroupID));
            return scenarios.Select(c => Transform(c)).ToList();
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

        /// <summary>
        /// Delete NodeCache data by ScenarioId
        /// </summary>
        public void ClearNodeCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            CacheTracker cacheTracker = new CacheTracker();
            cacheTracker.NodeCacheStale = true;
            ImplementScenarioChanges(scenarioId, cacheTracker);
        }

        /*
        /// <summary>
        /// Delete NodeCache data by ScenarioID and FragmentID
        /// </summary>
        public void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _NodeCacheService.ClearNodeCacheByScenarioAndFragment(scenarioId, fragmentId);
            _unitOfWork.SaveChanges();
        }
         * */

        /// <summary>
        /// Delete ScoreCache data by ScenarioId
        /// </summary>
        public void ClearScoreCacheByScenario(int scenarioId)
        {
            //_unitOfWork.SetAutoDetectChanges(false); 
            CacheTracker cacheTracker = new CacheTracker();
            cacheTracker.ScoreCacheStale = true;
            ImplementScenarioChanges(scenarioId, cacheTracker);
            //_unitOfWork.SetAutoDetectChanges(true);
        }

        /*
        /// <summary>
        /// Delete ScoreCache data by ScenarioID and FragmentID
        /// </summary>
        public void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _ScoreCacheService.ClearScoreCacheByScenarioAndFragment(scenarioId, fragmentId);
            _unitOfWork.SaveChanges();
        }
        */
        /// <summary>
        /// Delete ScoreCache data by ScenarioID and LCIAMethodID
        /// </summary>
        public void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId, int lciaMethodId)
        {
            CacheTracker cacheTracker = new CacheTracker();
            cacheTracker.LCIAMethodsStale.Add(lciaMethodId);
            ImplementScenarioChanges(scenarioId, cacheTracker);
        }

        private void ImplementScenarioChanges(int scenarioId, CacheTracker cacheTracker)
        {
            var sw = new CounterTimer();
            sw.CStart();
            // first, we do the most surgical operations: clear LCIA method-specific results
            _unitOfWork.SetAutoDetectChanges(false);
            if (cacheTracker.LCIAMethodsStale != null)
            {
                cacheTracker.Recompute = true; // set this explicitly
                foreach (int method in cacheTracker.LCIAMethodsStale)
                {
                    _ScoreCacheService.ClearScoreCacheByScenarioAndLCIAMethod(scenarioId, method);
                }
            }
            sw.Click("methods");
            if (cacheTracker.NodeCacheStale)
            {
                // next, clear all computed fragment scores
                _ScoreCacheService.ClearScoreCacheForSubFragments(scenarioId);
                _NodeCacheService.ClearNodeCacheByScenario(scenarioId);
            }
            sw.Click("node");
            if (cacheTracker.ScoreCacheStale)
                _ScoreCacheService.ClearScoreCacheByScenario(scenarioId);

            sw.Click("score");
            _unitOfWork.SaveChanges(); // update database with changes
            _unitOfWork.SetAutoDetectChanges(true);
            sw.Click("save");
            if (cacheTracker.Recompute)
            {
                int tlf = _ScenarioService.Query(k => k.ScenarioID == scenarioId).Select(k => k.TopLevelFragmentID).First();
                _FragmentLCIAComputation.FragmentLCIAComputeSave(tlf, scenarioId);
            }
            sw.CStop();
            return;
        }

        private ScenarioResource CheckTopLevelFragment(ScenarioResource scenario)
        {
            if (_FragmentService.Query(k => k.FragmentID == scenario.TopLevelFragmentID) == null)
                return null;

            var term = _FragmentFlowService.GetInFlow(scenario.TopLevelFragmentID);
            if (scenario.ReferenceFlowID == 0)
            {
                scenario.ReferenceFlowID = term.FlowID;
            }
            else
            {
                // alternative ReferenceFlowID is allowed only if it can be converted to the same units as term.FlowID
                var conv = _FlowFlowPropertyService.FlowConv(scenario.ReferenceFlowID, term.FlowID);
                if (conv == 0)
                    return null; // must be commensurable
                // else - nothing required
            }
            scenario.ReferenceDirection = Enum.GetName(typeof(DirectionEnum), (DirectionEnum)term.DirectionID);
            return scenario;
        }

        public int AddScenario(ScenarioResource postScenario, int scenarioGroupId)
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
            postScenario = CheckTopLevelFragment(postScenario);
            if (postScenario == null)
                return -1; // -1 should generate a 400 or 415 http error
            
            if (postScenario.Name == null)
                postScenario.Name = "User-Generated Scenario";
            if (postScenario.ActivityLevel == 0)
                postScenario.ActivityLevel = 1;
            postScenario.ScenarioGroupID = scenarioGroupId;
            Scenario newScenario = _ScenarioService.NewScenario(postScenario);
            _unitOfWork.SaveChanges();
            _FragmentLCIAComputation.FragmentLCIAComputeSave(newScenario.TopLevelFragmentID, newScenario.ScenarioID);
            return newScenario.ScenarioID;
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
                putScenario = CheckTopLevelFragment(putScenario);
                scenario = _ScenarioService.UpdateScenarioFlow(scenarioId, putScenario, ref cacheTracker);
            }
            if (scenario != null)
            {
                ImplementScenarioChanges(scenario.ScenarioID, cacheTracker);
                return true;
            }
            return false;
        }

        public void DeleteScenario(int scenarioId)
        {
            _ScenarioService.DeleteScenario(scenarioId);
            _unitOfWork.SaveChanges();
        }

        public void DeleteParam(int scenarioId, int paramId)
        {
            CacheTracker cacheTracker = new CacheTracker();
            int p_scenarioId = _ParamService.Query(k => k.ParamID == paramId)
                .Select(k => k.ScenarioID).First();
            if (p_scenarioId == scenarioId)
            {
                _ParamService.DeleteParam(paramId, ref cacheTracker);
                ImplementScenarioChanges(scenarioId, cacheTracker);
            }
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
            IEnumerable<Param> Ps = _ParamService.NewOrUpdateParam(scenarioId, postParam, ref cacheTracker);
            ImplementScenarioChanges(scenarioId, cacheTracker);
            return _ParamService.GetParamResource(Ps);
        }

        public IEnumerable<ParamResource> UpdateParam(int scenarioId, int paramId, ParamResource putParam)
        {
            CacheTracker cacheTracker = new CacheTracker();
            IEnumerable<Param> Ps = new List<Param>();
            if (_ParamService.Query(k => k.ParamID == paramId).Select(k => k.ScenarioID).First() == scenarioId)
                Ps = _ParamService.UpdateParam(paramId, putParam, ref cacheTracker);
            else
                Ps = _ParamService.NewOrUpdateParam(scenarioId, putParam, ref cacheTracker);
            ImplementScenarioChanges(scenarioId, cacheTracker);
            return _ParamService.GetParamResource(Ps);
        }


      #endregion  
    }
}
