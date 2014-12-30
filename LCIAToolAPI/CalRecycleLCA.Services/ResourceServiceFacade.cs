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
        private readonly ICategoryService _CategoryService;
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
        private readonly IDependencyParamService _DependencyParamService;
        [Inject]
        private readonly IFlowFlowPropertyService _FlowFlowPropertyService;
        [Inject]
        private readonly IFlowPropertyParamService _FlowPropertyParamService;
        [Inject]
        private readonly IProcessEmissionParamService _ProcessEmissionParamService;
        [Inject]
        private readonly ILCIAService _LCIAService;
        [Inject]
        private readonly ICharacterizationParamService _CharacterizationParamService;
        [Inject]
        private readonly IUnitOfWork _unitOfWork;
        //
        // Traversal and Computation components
        //
        [Inject]
        private readonly IFragmentTraversalV2 _FragmentTraversalV2;
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
                               ICategoryService categoryService,
                               IClassificationService classificationService,
                               IFragmentService fragmentService,
                               IFragmentFlowService fragmentFlowService,
                               IFragmentStageService fragmentStageService,   
                               IFragmentTraversalV2 fragmentTraversalV2,
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
                               IDependencyParamService dependencyParamService,
                               IFlowFlowPropertyService flowFlowPropertyService,
                               IFlowPropertyParamService flowPropertyParamService,
                               IProcessEmissionParamService processEmissionParamService,
                               ILCIAService lciaService,
                               ICharacterizationParamService characterizationParamService,
                               IUnitOfWork unitOfWork) 
        {
            _CategoryService = verifiedDependency(categoryService);
            _ClassificationService = verifiedDependency(classificationService);
            _FragmentService = verifiedDependency(fragmentService);
            _FragmentFlowService = verifiedDependency(fragmentFlowService);
            _FragmentStageService = verifiedDependency(fragmentStageService);
            _FragmentLCIAComputation = verifiedDependency(fragmentLCIAComputation);
            _FragmentTraversalV2 = verifiedDependency(fragmentTraversalV2);         
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
            _DependencyParamService = verifiedDependency(dependencyParamService);
            _FlowFlowPropertyService = verifiedDependency(flowFlowPropertyService);
            _FlowPropertyParamService = verifiedDependency(flowPropertyParamService);
            _ProcessEmissionParamService = verifiedDependency(processEmissionParamService);
            _LCIAService = verifiedDependency(lciaService);
            _CharacterizationParamService = verifiedDependency(characterizationParamService);
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
                ImpactCategoryID = TransformNullable(lm.ImpactCategoryID, "LCIAMethod.ImpactCategoryID"),
                ImpactIndicator = lm.ImpactIndicator,
                ReferenceYear = lm.ReferenceYear,
                Duration = lm.Duration,
                ImpactLocation = lm.ImpactLocation,
                IndicatorType = lm.IndicatorType.Name,
                Normalization = TransformNullable(lm.Normalization, "LCIAMethod.Normalization"),
                Weighting = TransformNullable(lm.Weighting, "LCIAMethod.Weighting"),
                UseAdvice = lm.UseAdvice,
                ReferenceFlowProperty = Transform(lm.FlowProperty)
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
            

            maxHL = classes.Max(c => Convert.ToInt32(c.Category.HierarchyLevel));
            categoryName = classes.Where(c => c.Category.HierarchyLevel == maxHL).Single().Category.Name;

            return new FlowResource
            {
                FlowID = f.FlowID,
                Name = f.Name,
                FlowTypeID = TransformNullable(f.FlowTypeID, "Flow.FlowTypeID"),
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

        public FragmentResource Transform(Fragment f)
        {
            return new FragmentResource
            {
                FragmentID = f.FragmentID,
                Name = f.Name,
                ReferenceFragmentFlowID = TransformNullable(f.ReferenceFragmentFlowID, "Fragment.ReferenceFragmentFlowID")
            };
        }

        public ProcessResource Transform(Process p, IList<int> emisProcesses)
        {
            return new ProcessResource
            {
                ProcessID = p.ProcessID,
                Name = p.Name,
                Geography = p.Geography,
                ProcessTypeID = TransformNullable(p.ProcessTypeID, "Process.ProcessTypeID"),
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
                DirectionID = TransformNullable(pf.DirectionID, "ProcessFlow.DirectionID"),
                VarName = pf.VarName,
                Magnitude = TransformNullable(pf.Magnitude, "ProcessFlow.Magnitude"),
                Result = TransformNullable(pf.Result, "ProcessFlow.Result"),
                STDev = TransformNullable(pf.STDev, "ProcessFlow.STDev")
            };
        }

        public DetailedLCIAResource Transform(LCIAModel m)
        {
            return new DetailedLCIAResource
            {
                //LCIAMethodID = TransformNullable(m.LCIAMethodID, "LCIAModel.LCIAMethodID"),
                FlowID = TransformNullable(m.FlowID, "LCIAModel.FlowID"),
                DirectionID = TransformNullable(m.DirectionID, "LCIAModel.DirectionID"),
                Quantity = Convert.ToDouble(m.Quantity),
                Factor = Convert.ToDouble(m.Factor),
                Result = Convert.ToDouble(m.LCIAResult)
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
                ReferenceDirectionID = s.DirectionID
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
            _FragmentTraversalV2.Traverse(fragmentID, scenarioID);
            var test = _FragmentFlowService.GetTerminatedFlows(fragmentID, scenarioID)
                .ToList();
            foreach (var ff in test)
                ff.FlowPropertyMagnitudes = _FlowFlowPropertyService.GetFlowPropertyMagnitudes(ff, scenarioID);

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
            if (flowtypeID == 0)
                return _FlowService.Query().Select()
                    .Select(f => Transform(f)).ToList();
            else
                return _FlowService.Query(f => f.FlowTypeID == flowtypeID).Select()
                    .Select(f => Transform(f)).ToList();
        }

        /// <summary>
        /// Get list of flows related to a fragment (via FragmentFlow)
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowResource objects</returns>
        public IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID)
        {
            //return GetFlows(typeof(FragmentFlow), fragmentID);
            return _FlowService.Query(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentID))
                .Include(x => x.FragmentFlows)
                .Select()
                .Select(f => Transform(f)).ToList();
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
            return _FlowPropertyService.Query()
                .Include(fp => fp.UnitGroup.UnitConversion)
                .Select()
                .Select(fp => Transform(fp)).ToList();
        }

        /// <summary>
        /// Get list of flow properties related to a process (via FlowFlowProperty and ProcessFlow)
        /// </summary>
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID)
        {
            //IEnumerable<FlowProperty> flowProperties = _FlowPropertyService.Query()
            //    .Include(fp => fp.UnitGroup.UnitConversion)
            //    .Filter(fp => fp.Flows.Any(f => f.ProcessFlows.Any(pf => pf.ProcessID == processID)))
            //    .Get();
            /*var pf_flowproperties = _ProcessFlowService.Query(pf => pf.ProcessID == processID)
                .Include(pf => pf.Flow.FlowFlowProperties)
                .GroupBy(x => x.Flow.FlowFlowProperties.Select(f => f.FlowPropertyID))
                .Select(x => x.Flow.FlowFlowProperties.Select(f => f.FlowPropertyID));
              */  
            var flowProperties = _FlowPropertyService
                .Query(fp => fp.Flows.Any(f => f.ProcessFlows.Any(pf => pf.ProcessID == processID)))
                .Include(fp => fp.UnitGroup.UnitConversion)
                //.Include(fp => fp.Flows.Select(p => p.ProcessFlows)) 
                .Select().ToList();
            return flowProperties.Select(fp => Transform(fp)).ToList();
        }

        /// <summary>
        /// Get FlowProperty data related to fragment and transform to API resource model
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowPropertyResource objects</returns>
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID)
        {
            var flowProperties = _FlowPropertyService
                .Query(fp => fp.Flows.Any(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentID)))
                .Include(fp => fp.UnitGroup.UnitConversion)
                //.Include(fp => fp.Flows.Select(f => f.FragmentFlows))
                .Select().ToList();
            return flowProperties.Select(fp => Transform(fp)).ToList();
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
        public IEnumerable<ProcessResource> GetProcesses(int? flowTypeID = null)
        {

            var emisProcesses = _ProcessService.Query(p => p.ProcessFlows.Any(pf => pf.Flow.FlowTypeID == 2))
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
        public LCIAResultResource GetProcessLCIAResult(int processID, int lciaMethodID, int scenarioID = Scenario.MODEL_BASE_CASE_ID) {
            LCIAMethod lciaMethod = _LciaMethodService.Find(lciaMethodID);
            if (lciaMethod == null)
            {
                // TODO: figure how to handle this sort of error
                return null;
            }
            else
            {
                IEnumerable<InventoryModel> inventory = _LCIAComputation.ComputeProcessLCI(processID, scenarioID);
                IEnumerable<LCIAModel> lciaDetail = _LCIAComputation.ComputeProcessLCIA(inventory, lciaMethod, scenarioID)
                    .Where(l => String.IsNullOrEmpty(l.Geography));
                // var privacy_flag = _ProcessService.Query(p => p.ProcessID == processID)
                //     .Include(p => p.ILCDEntity.DataSource)
                //     .Select(p => p.ILCDEntity.DataSource.VisibilityID).First() == 2;
                var lciaScore = new AggregateLCIAResource
                    {
                        ProcessID = processID,
                        CumulativeResult = (double)lciaDetail.Sum(a => a.LCIAResult),
                        LCIADetail = (_ProcessService.IsPrivate(processID)
                            ? new List<DetailedLCIAResource>()
                            : lciaDetail.Select(m => Transform(m)).ToList())
                    };  
                var lciaResult = new LCIAResultResource
                {
                    LCIAMethodID = lciaMethodID,
                    ScenarioID = scenarioID,
                    LCIAScore = new List<AggregateLCIAResource>() { lciaScore }
                };
                return lciaResult;
            }
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
            IEnumerable<FragmentLCIAModel> aggResults = _FragmentLCIAComputation.FragmentLCIA(fragmentID, scenarioID, lciaMethodID)
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
        }

        /// <summary>
        /// Execute Fragment LCIA and return computation results in FragmentLCIAResource objects
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
        /// Get scenario types, with optional ScenarioGroup argument to constrain the ScenarioIDs returned
        /// </summary>
        /// <returns>List of ScenarioResource objects</returns>
        public IEnumerable<ScenarioResource> GetScenarios()
        {
            IEnumerable<Scenario> scenarios = _ScenarioService.Queryable();
            return scenarios.Select(c => Transform(c)).ToList();
        }

        public IEnumerable<ScenarioResource> GetScenarios(int userGroupID)
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
            _NodeCacheService.ClearNodeCacheByScenario(scenarioId);
            _unitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete NodeCache data by ScenarioID and FragmentID
        /// </summary>
        public void ClearNodeCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _NodeCacheService.ClearNodeCacheByScenarioAndFragment(scenarioId, fragmentId);
            _unitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete ScoreCache data by ScenarioId
        /// </summary>
        public void ClearScoreCacheByScenario(int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            _ScoreCacheService.ClearScoreCacheByScenario(scenarioId);
            _unitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete ScoreCache data by ScenarioID and FragmentID
        /// </summary>
        public void ClearScoreCacheByScenarioAndFragment(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int fragmentId = 0)
        {
            _ScoreCacheService.ClearScoreCacheByScenarioAndFragment(scenarioId, fragmentId);
            _unitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete ScoreCache data by ScenarioID and LCIAMethodID
        /// </summary>
        public void ClearScoreCacheByScenarioAndLCIAMethod(int scenarioId = Scenario.MODEL_BASE_CASE_ID, int lciaMethodId = 0)
        {
            throw new NotImplementedException("need a way to repopulate cache afterwards-- maybe make this fragment-specific");
            //_ScoreCacheService.ClearScoreCacheByScenarioAndLCIAMethod(scenarioId, lciaMethodId);
            //_unitOfWork.SaveChanges();
        }

        public int AddScenario(string addScenarioJSON, int scenarioGroupId)
        {
            /// this function is a big old mess.  Needs to generate a new ScenarioID, not accept it as input. 
            /// Needs to create only one scenario and not a list.
            // Parse JSON into dynamic object
            JObject jsonScenarios = JObject.Parse(addScenarioJSON);

            List<Scenario> scenarios = new List<Scenario>();

            // add each scenario to a list
            foreach (var jsonScenario in jsonScenarios["scenarios"])
            {
                scenarios.Add(new Scenario()
                {
                    ScenarioGroupID = Convert.ToInt32((string)jsonScenario["scenarioGroupID"]),
                    TopLevelFragmentID = Convert.ToInt32((string)jsonScenario["topLevelFragmentID"]),
                    ActivityLevel = Convert.ToDouble((string)jsonScenario["activityLevel"]),
                    Name = (string)jsonScenario["name"],
                    FlowID = Convert.ToInt32((string)jsonScenario["flowID"]),
                    DirectionID = Convert.ToInt32((string)jsonScenario["directionID"])
                });
            }

            foreach (var scenario in scenarios)
            {
                scenario.ObjectState = ObjectState.Added;
            }

            _ScenarioService.InsertGraphRange(scenarios);
            _unitOfWork.SaveChanges();
            return scenarios.First().ScenarioID;

        }

        public void UpdateScenario(string updateScenarioJSON, int scenarioGroupId)
        {
            // Parse JSON into dynamic object
            JObject jsonScenarios = JObject.Parse(updateScenarioJSON);

            List<Scenario> scenarios = new List<Scenario>();

            // add each scenario to a list
            foreach (var jsonScenario in jsonScenarios["scenarios"])
            {
                scenarios.Add(new Scenario()
                {
                    ScenarioID = Convert.ToInt32((string)jsonScenario["scenarioID"]),
                    ScenarioGroupID = Convert.ToInt32((string)jsonScenario["scenarioGroupID"]),
                    TopLevelFragmentID = Convert.ToInt32((string)jsonScenario["topLevelFragmentID"]),
                    ActivityLevel = Convert.ToDouble((string)jsonScenario["activityLevel"]),
                    Name = (string)jsonScenario["name"],
                    FlowID = Convert.ToInt32((string)jsonScenario["flowID"]),
                    DirectionID = Convert.ToInt32((string)jsonScenario["directionID"])
                });
            }

            foreach (var scenario in scenarios)
            {
                scenario.ObjectState = ObjectState.Modified;
                _ScenarioService.InsertOrUpdateGraph(scenario);
            }
            _unitOfWork.SaveChanges();

        }

        public void DeleteScenario(string deleteScenarioJSON)
        {
            // Parse JSON into dynamic object
            JObject jsonScenarios = JObject.Parse(deleteScenarioJSON);

            List<Scenario> scenarios = new List<Scenario>();

            // add each scenario to a list
            foreach (var jsonScenario in jsonScenarios["scenarios"])
            {
                scenarios.Add(new Scenario()
                {
                    ScenarioID = Convert.ToInt32((string)jsonScenario["scenarioID"])
                });
            }

            foreach (var scenario in scenarios)
            {
                scenario.ObjectState = ObjectState.Deleted;
                _ScenarioService.Delete(scenario.ScenarioID);
            }
            _unitOfWork.SaveChanges();

        }

        public void DeleteParam(string deleteParamJSON)
        {
            // Parse JSON into dynamic object
            JObject jsonParams = JObject.Parse(deleteParamJSON);

            List<Param> paramList = new List<Param>();

            // add each param to a list
            foreach (var jsonParam in jsonParams["paramList"])
            {
                paramList.Add(new Param()
                {
                    ParamID = Convert.ToInt32((string)jsonParam["paramID"]),

                });
            }

            foreach (var param in paramList)
            {
                param.ObjectState = ObjectState.Deleted;
                _ParamService.Delete(param.ParamID);
            }
            _unitOfWork.SaveChanges();

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

        public void AddParam(string addParamJSON)
        {
            // Parse JSON into dynamic object
            JObject jsonParams = JObject.Parse(addParamJSON);

            List<ParamResource> paramList = new List<ParamResource>();

            // add each param resource to a list
            foreach (var jsonParam in jsonParams["params"])
            {
                paramList.Add(new ParamResource()
                {
                    //ParamID = Convert.ToInt32((string)jsonParam["paramID"]),
                    ParamTypeID = Convert.ToInt32((string)jsonParam["paramTypeID"]),
                    ScenarioID = Convert.ToInt32((string)jsonParam["scenarioID"]),
                    Name = (string)jsonParam["name"],
                    Value = float.Parse((string)jsonParam["value"]),
                    FragmentFlowID = Convert.ToInt32((string)jsonParam["fragmentFlowID"]),
                    FlowID = Convert.ToInt32((string)jsonParam["flowID"]),
                    FlowPropertyID = Convert.ToInt32((string)jsonParam["flowPropertyID"]),
                    ProcessID = Convert.ToInt32((string)jsonParam["processID"]),
                    LCIAMethodID = Convert.ToInt32((string)jsonParam["lciaMethodID"])
                });
            }

            foreach (var paramResource in paramList)
            {

                Param param = new Param();
                DependencyParam dParam = new DependencyParam();
                FlowPropertyParam fpParam = new FlowPropertyParam();
                ProcessEmissionParam peParam = new ProcessEmissionParam();
                CharacterizationParam cParam = new CharacterizationParam();

                switch (paramResource.ParamTypeID)
                {
                    case 1:
                        {
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;
                            param.ObjectState = ObjectState.Added;
                            _ParamService.InsertOrUpdateGraph(param);
                            _unitOfWork.SaveChanges();
                            int paramId = param.ParamID;

                            dParam.ParamID = paramId;
                            dParam.FragmentFlowID = Convert.ToInt32(paramResource.FragmentFlowID);
                            dParam.Value = paramResource.Value;
                            dParam.ObjectState = ObjectState.Added;
                            _DependencyParamService.InsertOrUpdateGraph(dParam);
                            break;
                        }
                    case 2:
                        {
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;
                            param.ObjectState = ObjectState.Added;
                            _ParamService.InsertOrUpdateGraph(param);
                            _unitOfWork.SaveChanges();
                            int paramId = param.ParamID;

                            dParam.ParamID = paramId;
                            dParam.FragmentFlowID = Convert.ToInt32(paramResource.FragmentFlowID);
                            dParam.Value = paramResource.Value;

                            dParam.ObjectState = ObjectState.Added;
                            _DependencyParamService.InsertOrUpdateGraph(dParam);
                            break;
                        }

                    case 3:
                        {
                            break;
                        }

                    case 4:
                        {
                            int flowFlowPropertyID = _FlowFlowPropertyService.Queryable()
                                .Where(x => x.FlowID == paramResource.FlowID && x.FlowPropertyID == paramResource.FlowPropertyID)
                                .FirstOrDefault()
                                .FlowFlowPropertyID;

                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;
                            param.ObjectState = ObjectState.Added;
                            _ParamService.InsertOrUpdateGraph(param);
                            _unitOfWork.SaveChanges();
                            int paramId = param.ParamID;

                            fpParam.ParamID = paramId;
                            fpParam.FlowFlowPropertyID = Convert.ToInt32(flowFlowPropertyID);

                            fpParam.ObjectState = ObjectState.Added;
                            _FlowPropertyParamService.InsertOrUpdateGraph(fpParam);
                            break;
                        }

                    case 5:
                        {
                            //pending db update
                            break;
                        }

                    case 6:
                        {
                            //pending db update
                            break;
                        }


                    case 8:
                        {
                            int processID = _ProcessFlowService.Queryable()
                                .Where(x => x.FlowID == paramResource.FlowID)
                                .FirstOrDefault()
                                .ProcessID;
                            int processFlowID = _ProcessFlowService.Queryable()
                                .Where(x => x.FlowID == paramResource.FlowID)
                                .FirstOrDefault()
                                .ProcessFlowID;

                            param.ParamID = paramResource.ParamID;
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;
                            param.ObjectState = ObjectState.Added;
                            _ParamService.InsertOrUpdateGraph(param);
                            _unitOfWork.SaveChanges();
                            int paramId = param.ParamID;

                            peParam.ParamID = paramId;
                            peParam.ProcessFlowID = Convert.ToInt32(processFlowID);

                            peParam.ObjectState = ObjectState.Added;
                            _ProcessEmissionParamService.InsertOrUpdateGraph(peParam);
                            break;
                        }

                    case 9:
                        {
                            int lciaID = _LCIAService.Queryable()
                                .Where(x => x.FlowID == paramResource.FlowID)
                                .FirstOrDefault()
                                .LCIAID;

                            param.ParamID = paramResource.ParamID;
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;
                            param.ObjectState = ObjectState.Added;
                            _ParamService.InsertOrUpdateGraph(param);
                            _unitOfWork.SaveChanges();
                            int paramId = param.ParamID;

                            cParam.ParamID = paramId;
                            cParam.LCIAID = Convert.ToInt32(lciaID);

                            cParam.ObjectState = ObjectState.Added;
                            _CharacterizationParamService.InsertOrUpdateGraph(cParam);
                            break;
                        }


                }
            }
                _unitOfWork.SaveChanges();

    }

        public void UpdateParam(string updateParamJSON)
        {
            // Parse JSON into dynamic object
            JObject jsonParams = JObject.Parse(updateParamJSON);

            List<ParamResource> paramList = new List<ParamResource>();

            // add each param resource to a list
            foreach (var jsonParam in jsonParams["params"])
            {
                paramList.Add(new ParamResource()
                {
                    ParamID = Convert.ToInt32((string)jsonParam["paramID"]),
                    ParamTypeID = Convert.ToInt32((string)jsonParam["paramTypeID"]),
                    ScenarioID = Convert.ToInt32((string)jsonParam["scenarioID"]),
                    Name = (string)jsonParam["name"],
                    Value = float.Parse((string)jsonParam["value"]),
                    FragmentFlowID = Convert.ToInt32((string)jsonParam["fragmentFlowID"]),
                    FlowID = Convert.ToInt32((string)jsonParam["flowID"]),
                    FlowPropertyID = Convert.ToInt32((string)jsonParam["flowPropertyID"]),
                    ProcessID = Convert.ToInt32((string)jsonParam["processID"]),
                    LCIAMethodID = Convert.ToInt32((string)jsonParam["lciaMethodID"])
                });
            }

            foreach (var paramResource in paramList)
            {

                Param param = new Param();
                DependencyParam dParam = new DependencyParam();
                FlowPropertyParam fpParam = new FlowPropertyParam();
                ProcessEmissionParam peParam = new ProcessEmissionParam();
                CharacterizationParam cParam = new CharacterizationParam();

                switch (paramResource.ParamTypeID)
                {
                    case 1:
                        {

                            param.ParamID = paramResource.ParamID;
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;

                            dParam.ParamID = paramResource.ParamID;
                            dParam.FragmentFlowID = Convert.ToInt32(paramResource.FragmentFlowID);
                            dParam.Value = paramResource.Value;

                            param.ObjectState = ObjectState.Modified;
                            _ParamService.InsertOrUpdateGraph(param);
                            dParam.ObjectState = ObjectState.Modified;
                            _DependencyParamService.InsertOrUpdateGraph(dParam);

                            break;
                        }
                    case 2:
                        {
                            param.ParamID = paramResource.ParamID;
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;

                            dParam.ParamID = paramResource.ParamID;
                            dParam.FragmentFlowID = Convert.ToInt32(paramResource.FragmentFlowID);
                            dParam.Value = paramResource.Value;

                            param.ObjectState = ObjectState.Modified;
                            _ParamService.InsertOrUpdateGraph(param);
                            dParam.ObjectState = ObjectState.Modified;
                            _DependencyParamService.InsertOrUpdateGraph(dParam);
                            break;
                        }

                    case 3:
                        {
                            break;
                        }

                    case 4:
                        {
                            int flowFlowPropertyID = _FlowFlowPropertyService.Queryable()
                                .Where(x => x.FlowID == paramResource.FlowID && x.FlowPropertyID == paramResource.FlowPropertyID)
                                .FirstOrDefault()
                                .FlowFlowPropertyID;

                            param.ParamID = paramResource.ParamID;
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;

                            fpParam.ParamID = paramResource.ParamID;
                            fpParam.FlowFlowPropertyID = Convert.ToInt32(flowFlowPropertyID);

                            param.ObjectState = ObjectState.Modified;
                            _ParamService.InsertOrUpdateGraph(param);
                            fpParam.ObjectState = ObjectState.Modified;
                            _FlowPropertyParamService.InsertOrUpdateGraph(fpParam);
                            break;
                        }

                    case 8:
                        {
                            int processID = _ProcessFlowService.Queryable()
                                .Where(x => x.FlowID == paramResource.FlowID)
                                .FirstOrDefault()
                                .ProcessID;
                            int processFlowID = _ProcessFlowService.Queryable()
                                .Where(x => x.FlowID == paramResource.FlowID)
                                .FirstOrDefault()
                                .ProcessFlowID;

                            param.ParamID = paramResource.ParamID;
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;

                            peParam.ParamID = paramResource.ParamID;
                            peParam.ProcessFlowID = Convert.ToInt32(processFlowID);

                            param.ObjectState = ObjectState.Modified;
                            _ParamService.InsertOrUpdateGraph(param);
                            peParam.ObjectState = ObjectState.Modified;
                            _ProcessEmissionParamService.InsertOrUpdateGraph(peParam);
                            break;
                        }

                    case 9:
                        {
                            int lciaID = _LCIAService.Queryable()
                                .Where(x => x.FlowID == paramResource.FlowID)
                                .FirstOrDefault()
                                .LCIAID;

                            param.ParamID = paramResource.ParamID;
                            param.ParamTypeID = paramResource.ParamTypeID;
                            param.ScenarioID = paramResource.ScenarioID;
                            param.Name = paramResource.Name;

                            cParam.ParamID = paramResource.ParamID;
                            cParam.LCIAID = Convert.ToInt32(lciaID);

                            param.ObjectState = ObjectState.Modified;
                            _ParamService.InsertOrUpdateGraph(param);
                            cParam.ObjectState = ObjectState.Modified;
                            _CharacterizationParamService.InsertOrUpdateGraph(cParam);
                            break;
                        }


                }
            }
               

          
        
    }

      #endregion  
}
}
