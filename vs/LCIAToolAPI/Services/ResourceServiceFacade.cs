using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Entities.Models;
using Ninject;
using System.Runtime.CompilerServices;

namespace Services {
    /// <summary>
    /// Web API Services Facade
    /// Gets LcaDataModel entities and transforms them to web API resources.
    /// Executes Traversal and Computation as needed.
    /// </summary>
    public class ResourceServiceFacade : IResourceServiceFacade {
        //
        // LcaDataModel services
        //
        [Inject]
        private readonly IService<Category> _CategoryService;
        [Inject]
        private readonly IService<Fragment> _FragmentService;
        [Inject]
        private readonly IService<FragmentFlow> _FragmentFlowService;
        [Inject]
        private readonly IService<Flow> _FlowService;
        [Inject]
        private readonly IService<FlowProperty> _FlowPropertyService;
        [Inject]
        private readonly IService<FlowType> _FlowTypeService;
        [Inject]
        private readonly IService<ImpactCategory> _ImpactCategoryService;
        [Inject]
        private readonly IService<LCIAMethod> _LciaMethodService;
        [Inject]
        private readonly IService<Process> _ProcessService;
        [Inject]
        private readonly IService<ProcessFlow> _ProcessFlowService;
        [Inject]
        private readonly IService<Scenario> _ScenarioService;
        //
        // Traversal and Computation components
        //
        [Inject]
        private readonly IFragmentTraversalV2 _FragmentTraversalV2;
        [Inject]
        private readonly IFragmentLCIAComputation _FragmentLCIAComputation;
        [Inject]
        private readonly ILCIAComputationV2 _LCIAComputation;

        private T verifiedDependency<T>(T dependency) where T : class {
            if (dependency == null) {
                throw new ArgumentNullException("dependency", String.Format("Type: {0}", dependency.GetType().ToString()));
            }
            else {
                return dependency;
            }
        }

        /// <summary>
        /// Constructor for use with Ninject dependency injection
        /// </summary>
        public ResourceServiceFacade( 
                               IService<Category> categoryService,
                               IService<Fragment> fragmentService,
                               IService<FragmentFlow> fragmentFlowService,
                               IFragmentTraversalV2 fragmentTraversalV2,
                               IFragmentLCIAComputation fragmentLCIAComputation,
                               IService<Flow> flowService,
                               IService<FlowProperty> flowPropertyService,
                               IService<FlowType> flowTypeService,
                               IService<ImpactCategory> impactCategoryService,
                               ILCIAComputationV2 lciaComputation,
                               IService<LCIAMethod> lciaMethodService,
                               IService<Process> processService,
                               IService<ProcessFlow> processFlowService,
                               IService<Scenario> scenarioService) 
        {
            _CategoryService = verifiedDependency(categoryService);
            _FragmentService = verifiedDependency(fragmentService);
            _FragmentFlowService = verifiedDependency(fragmentFlowService);
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
        }

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
        private int TransformNullable(int? propVal, string propName) {
            if (propVal == null) {
                throw new ArgumentNullException(propName);
            }
            else {
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
        private bool TransformNullable(bool? propVal, string propName) {
            if (propVal == null) {
                throw new ArgumentNullException(propName);
            }
            else {
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
        private double TransformNullable(double? propVal, string propName) {
            if (propVal == null) {
                throw new ArgumentNullException(propName);
            }
            else {
                return Convert.ToDouble(propVal);
            }
        }

        // Transformation methods

        /// <summary>
        /// Transform an LCIAMethod object to a LCIAMethodResource object
        /// </summary>
        /// <param name="lm"></param>
        /// <returns></returns>
        public LCIAMethodResource Transform(LCIAMethod lm) {
            return new LCIAMethodResource {
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

        private ICollection<FlowPropertyMagnitude> GetFlowPropertyMagnitudes(FragmentFlow ff, int scenarioID) {
            IEnumerable<FlowFlowProperty> ffpData = ff.Flow.FlowFlowProperties;
            IEnumerable<NodeCache> ncData = ff.NodeCaches;

            NodeCache nodeCache = ncData.Where(nc => nc.ScenarioID == scenarioID).FirstOrDefault();
            if (nodeCache == null) {
                return null;
            }
            else {
                double flowMagnitude = Convert.ToDouble(nodeCache.FlowMagnitude);
                return ffpData.Select(ffp =>
                        new FlowPropertyMagnitude {
                            FlowPropertyID = Convert.ToInt32(ffp.FlowPropertyID),
                            Magnitude = flowMagnitude * Convert.ToDouble(ffp.MeanValue)
                        }).ToList();
            }

        }

        public FragmentFlowResource Transform(FragmentFlow ff, int scenarioID) {
            int? nullID = null;
            return new FragmentFlowResource {
                FragmentFlowID = ff.FragmentFlowID,
                Name = ff.Name,
                ShortName = ff.ShortName,
                NodeTypeID = TransformNullable(ff.NodeTypeID, "FragmentFlow.NodeTypeID"),
                DirectionID = TransformNullable(ff.DirectionID, "FragmentFlow.DirectionID"),
                FlowID = ff.FlowID,
                ParentFragmentFlowID = ff.ParentFragmentFlowID,
                ProcessID = (ff.NodeTypeID == 1) ? ff.FragmentNodeProcesses.FirstOrDefault().ProcessID : nullID,
                SubFragmentID = (ff.NodeTypeID == 2) ? ff.FragmentNodeFragments.FirstOrDefault().SubFragmentID : nullID,
                FlowPropertyMagnitudes = (ff.FlowID == null) ? null : GetFlowPropertyMagnitudes(ff, scenarioID)
            };
        }

        /// <summary>
        /// Transform Flow to FlowResource by joining with Category data
        /// </summary>
        /// <param name="f">Instance of Flow</param>
        /// <param name="categories">all categories</param>
        /// <returns>Instance of FlowResource</returns>
        public FlowResource Transform(Flow f, IEnumerable<Category> categories) {
          
            var joinResult = f.ILCDEntity.Classifications.Join(categories, cl => cl.CategoryID, cat => cat.CategoryID, 
                (cl, cat) => new {
                    ClassificationID = cl.ClassificationID,
                    Name = cat.Name,
                    HierarchyLevel = cat.HierarchyLevel
                });
            int? maxHL = joinResult.Max(j => j.HierarchyLevel);
            string categoryName = joinResult.Where(j => j.HierarchyLevel == maxHL).Single().Name;

            return new FlowResource {
                FlowID = f.FlowID,
                Name = f.Name,
                FlowTypeID = TransformNullable(f.FlowTypeID, "Flow.FlowTypeID"),
                ReferenceFlowPropertyID = TransformNullable(f.ReferenceFlowProperty, "Flow.ReferenceFlowProperty"),
                CASNumber = f.CASNumber,
                Category = categoryName
            };
        }

        public FlowPropertyResource Transform(FlowProperty fp) {
            string unitName = "";
            if (fp.UnitGroup != null && fp.UnitGroup.UnitConversion != null) {
                unitName = fp.UnitGroup.UnitConversion.Unit;
            }
            return new FlowPropertyResource {
                FlowPropertyID = fp.FlowPropertyID,
                Name = fp.Name,
                ReferenceUnit = unitName
            };
        }

         public FragmentResource Transform(Fragment f) {
            return new FragmentResource {
                FragmentID = f.FragmentID,
                Name = f.Name,
                ReferenceFragmentFlowID = TransformNullable(f.ReferenceFragmentFlowID, "Fragment.ReferenceFragmentFlowID")
            };
        }

         public ProcessResource Transform(Process p) {
             return new ProcessResource {
                 ProcessID = p.ProcessID,
                 Name = p.Name,
                 Geography = p.Geography,
                 ProcessTypeID = TransformNullable(p.ProcessTypeID, "Process.ProcessTypeID"),
                 ReferenceTypeID = p.ReferenceTypeID,
                 ReferenceFlowID = p.ReferenceFlowID,
                 ReferenceYear = p.ReferenceYear,
                 Version = p.ILCDEntity.Version
             };
         }

         public ProcessFlowResource Transform(ProcessFlow pf, IEnumerable<Category> categories) {
             return new ProcessFlowResource {
                 ProcessFlowID = pf.ProcessFlowID,
                 Flow = Transform( pf.Flow, categories),
                 DirectionID = TransformNullable( pf.DirectionID, "ProcessFlow.DirectionID"),
                 VarName = pf.VarName,
                 Magnitude = TransformNullable(pf.Magnitude, "ProcessFlow.Magnitude"),
                 Result = TransformNullable(pf.Result, "ProcessFlow.Result"),
                 STDev = TransformNullable(pf.STDev, "ProcessFlow.STDev")
             };
         }

         public ProcessLCIAResultResource Transform(LCIAModel m) {
             return new ProcessLCIAResultResource {
                //LCIAMethodID = TransformNullable(m.LCIAMethodID, "LCIAModel.LCIAMethodID"),
                FlowID = TransformNullable(m.FlowID, "LCIAModel.FlowID"),
                DirectionID = TransformNullable(m.DirectionID, "LCIAModel.DirectionID"),
                Quantity = Convert.ToDouble(m.Result),
                Factor = (m.LCParamValue == null || m.LCParamValue == 0) ?
                        Convert.ToDouble(m.LCIAFactor) : 
                        Convert.ToDouble(m.LCParamValue),
                Result = Convert.ToDouble(m.ComputationResult)
             };
         }

         public FragmentFlowLCIAResource Transform(FragmentLCIAModel m) {
             return new FragmentFlowLCIAResource {
                 FragmentFlowID = TransformNullable(m.FragmentFlowID, "FragmentLCIAModel.FragmentFlowID"),
                 Result = Convert.ToDouble(m.ImpactScore)
             };
         }

        // Get list methods 

         /// <summary>
         /// Get LCIAMethodResource list with optional filter by ImpactCategory
         /// </summary>
         public IEnumerable<LCIAMethodResource> GetLCIAMethodResources(int? impactCategoryID = null) {
            IEnumerable<LCIAMethod> lciaMethods;
            Repository.RepositoryQuery<LCIAMethod> query = 
                _LciaMethodService.Query()
                .Include(lm => lm.IndicatorType)
                .Include(lm => lm.FlowProperty.UnitGroup.UnitConversion);

            if (impactCategoryID == null) {
                lciaMethods = query.Get();
            }
            else {
                lciaMethods = query.
                              Filter(d => d.ImpactCategoryID == impactCategoryID).Get();
            }
            return lciaMethods.Select(lm => Transform(lm)).ToList();
        }

        /// <summary>
        /// Execute fragment traversal and return computation results in FragmentFlowResource objects
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <param name="scenarioID">ScenarioID filter for NodeCache</param>
        /// <returns>List of FragmentFlowResource objects</returns>
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID = 0) {
            _FragmentTraversalV2.Traverse(fragmentID, scenarioID);
            IEnumerable<FragmentFlow> fragmentFlows = _FragmentFlowService.Query()
                                                        .Include(i => i.FragmentNodeFragments)
                                                        .Include(i => i.FragmentNodeProcesses)
                                                        .Include(i => i.NodeCaches)
                                                        .Include(i => i.Flow.FlowFlowProperties)
                                                        .Filter(q => q.FragmentID == fragmentID)
                                                        .Get();
            return fragmentFlows.Select(ff => Transform(ff, scenarioID)).ToList();
        }

        /// <summary>
        /// Get list of flows related to a fragment or a process
        /// </summary>
        /// <param name="relType">Relationship class type (FragmentFlow or ProcessFlow)</param>
        /// <param name="relID">ID of related fragment or process</param>
        /// <returns>List of FlowResource objects</returns>
        public IEnumerable<FlowResource> GetFlows(Type relType, int relID) {
            Repository.RepositoryQuery<Flow> flowQuery = _FlowService.Query().Include(f => f.ILCDEntity.Classifications);
            IEnumerable<Category> categories = _CategoryService.Query().Get();
            if (relType == typeof(FragmentFlow)) { 
                flowQuery = flowQuery.Filter(f => f.FragmentFlows.Any(ff => ff.FragmentID == relID));
            } else if (relType == typeof(ProcessFlow)) {
                flowQuery = flowQuery.Filter(f => f.ProcessFlows.Any(pf => pf.ProcessID == relID));
            }
            return flowQuery.Get().Select(f => Transform(f, categories)).ToList();
        }

        /// <summary>
        /// Get list of flows related to a fragment (via FragmentFlow)
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowResource objects</returns>
        public IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID) {
            return GetFlows(typeof(FragmentFlow), fragmentID);
        }

        /// <summary>
        /// Get list of processflow resources
        /// </summary>
        public IEnumerable<ProcessFlowResource> GetProcessFlows(int processID) {
            IEnumerable<ProcessFlow> processFlows = _ProcessFlowService.Query()
                .Include(pf => pf.Flow.ILCDEntity.Classifications)
                .Filter(pf => pf.ProcessID == processID)
                .Get();
            IEnumerable<Category> categories = _CategoryService.Query().Get();
            return processFlows.Select(pf => Transform(pf, categories)).ToList();

        }

        /// <summary>
        /// Get list of flow properties related to a process (via FlowFlowProperty and ProcessFlow)
        /// </summary>
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID) {
            IEnumerable<FlowProperty> flowProperties = _FlowPropertyService.Query()
                .Include(fp => fp.UnitGroup.UnitConversion)
                .Filter(fp => fp.Flows.Any(f => f.ProcessFlows.Any(pf => pf.ProcessID == processID)))
                .Get();
            return flowProperties.Select(fp => Transform(fp)).ToList();
        }

        /// <summary>
        /// Get FlowProperty data related to fragment and transform to API resource model
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowPropertyResource objects</returns>
        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentID) {
            IEnumerable<FlowProperty> flowProperties = _FlowPropertyService.Query()
                .Include(fp => fp.UnitGroup.UnitConversion)
                .Filter(fp => fp.Flows.Any(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentID)))
                .Get();
            return flowProperties.Select(fp => Transform(fp)).ToList();
        }

         /// <summary>
        /// Get Fragment data and transform to API resource model
        /// </summary>
        /// <returns>List of FragmentResource objects</returns>
        public IEnumerable<FragmentResource> GetFragmentResources()
        {
            IEnumerable<Fragment> fragments = _FragmentService.Query().Get();
            return fragments.Select(f => Transform(f)).ToList();
        }

        /// <summary>
        /// Get a Fragment  and transform to API resource model
        /// </summary>
        /// <param name="fragmentID">FragmentID</param>
        /// <returns>FragmentResource</returns>
        public FragmentResource GetFragmentResource(int fragmentID) {
            Fragment fragment = _FragmentService.FindById(fragmentID);
            if (fragment == null) {
                // error handling deferred to controller
                return null;
            }
            else {
                return Transform(fragment);
            }
        }

        /// <summary>
        /// Get Process data and transform to API resource model
        /// </summary>
        /// <param name="flowTypeID">Optional process flow type filter</param>
        /// <returns>List of ProcessResource objects</returns>
        public IEnumerable<ProcessResource> GetProcesses(int? flowTypeID=null) {
            var query = _ProcessService.Query().Include(p => p.ILCDEntity);
            if (flowTypeID != null) {
                query = query.Filter(p => p.ProcessFlows.Any(pf => pf.Flow.FlowTypeID == flowTypeID));
            }
            IEnumerable<Process> pData = query.Get();
            return pData.Select(p => Transform(p)).ToList();
        }

        /// <summary>
        /// Get ImpactCategory data and transform to API resource model
        /// Omit categories that are not related to any LCIAMethod
        /// </summary>
        /// <returns>List of ImpactCategoryResource objects</returns>
        public IEnumerable<ImpactCategoryResource> GetImpactCategories() {
            IEnumerable<ImpactCategory> data = _ImpactCategoryService.Query()
                .Filter(i => i.LCIAMethods.Count() > 0)
                .Get();
            return data.Select(d => new ImpactCategoryResource {
                ImpactCategoryID = d.ID,
                Name = d.Name
            }).ToList();
        }

        /// <summary>
        /// Execute Process LCIA and return computation results in LCIAResultResource objects
        /// Work around problem in LCIA computation: should be filtering out LCIA with Geography 
        /// </summary>
        /// <returns>LCIAResultResource or null if lciaMethodID not found</returns> 
        public LCIAResultResource GetLCIAResultResource(int processID, int lciaMethodID, int scenarioID = 0) {
            LCIAMethod lciaMethod = _LciaMethodService.FindById(lciaMethodID);
            if (lciaMethod == null) {
                // TODO: figure how to handle this sort of error
                return null;
            }
            else {
                IEnumerable<InventoryModel> inventory = _LCIAComputation.ComputeProcessLCI(processID, scenarioID);
                IEnumerable<LCIAModel> lciaResults = _LCIAComputation.ComputeProcessLCIA(inventory, lciaMethod, scenarioID)
                    .Where(l => String.IsNullOrEmpty(l.Geography));
                LCIAResultResource lciaResult = new LCIAResultResource {
                    LCIAMethodID = lciaMethodID,
                    ProcessLCIAResults = lciaResults.Select(m => Transform(m)).ToList()
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
        public FragmentLCIAResource GetFragmentLCIAResults(int fragmentID, int lciaMethodID, int scenarioID = 0) {
            IEnumerable<FragmentLCIAModel> results = _FragmentLCIAComputation.ComputeFragmentLCIA(fragmentID, scenarioID, lciaMethodID);
            IEnumerable<FragmentLCIAModel> aggResults = results
                .GroupBy(r => r.FragmentFlowID)
                .Select(group => new FragmentLCIAModel
                {
                    FragmentFlowID = group.Key,
                    ImpactScore = group.Sum(a => a.ImpactScore)
                });
            FragmentLCIAResource lciaResult = new FragmentLCIAResource {
                ScenarioID = scenarioID,
                FragmentFlowLCIAResults = aggResults.Select(r => Transform(r)).ToList()
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
        public IEnumerable<FragmentLCIAResource> GetFragmentLCIAResultsAllScenarios(int fragmentID, int lciaMethodID, int scenarioGroupID = 1) {
            IEnumerable<Scenario> scenarios = _ScenarioService.Query().Filter(s => s.ScenarioGroupID == scenarioGroupID).Get();
            return scenarios.Select(s => GetFragmentLCIAResults(fragmentID, lciaMethodID, s.ScenarioID)).ToList();
        }

        /// <summary>
        /// Get FlowType data and transform to API resource model
        /// </summary>
        /// <returns>List of FlowTypeResource objects</returns>
        public IEnumerable<FlowTypeResource> GetFlowTypes() {
            IEnumerable<FlowType> data = _FlowTypeService.Query().Get();
            return data.Select(d => new FlowTypeResource {
                FlowTypeID = d.ID,
                Name = d.Name
            }).ToList();
        }

    }
}
