using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Entities.Models;
using Ninject;

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
        private readonly IService<Fragment> _FragmentService;
        [Inject]
        private readonly IService<FragmentFlow> _FragmentFlowService;
        [Inject]
        private readonly IService<Flow> _FlowService;
        [Inject]
        private readonly IService<FlowProperty> _FlowPropertyService;
        [Inject]
        private readonly IService<LCIAMethod> _LciaMethodService;
        [Inject]
        private readonly IService<Process> _ProcessService;
        //
        // Traversal and Computation components
        //
        [Inject]
        private readonly IFragmentTraversalV2 _FragmentTraversalV2;

        /// <summary>
        /// Constructor for use with Ninject dependency injection
        /// </summary>
        /// <param name="fragmentService"></param>
        /// <param name="fragmentFlowService"></param>
        /// <param name="fragmentTraversalV2"></param>
        /// <param name="flowService"></param>
        /// <param name="flowPropertyService"></param>
        /// <param name="lciaMethodService"></param>
        /// <param name="processService"></param>
        public ResourceServiceFacade( IService<Fragment> fragmentService,
                               IService<FragmentFlow> fragmentFlowService,
                               IFragmentTraversalV2 fragmentTraversalV2,
                               IService<Flow> flowService,
                               IService<FlowProperty> flowPropertyService,
                               IService<LCIAMethod> lciaMethodService,
                               IService<Process> processService ) 
        {
            if (fragmentService == null) {
                throw new ArgumentNullException("fragmentService");
            }
            _FragmentService = fragmentService;
            if (fragmentFlowService == null) {
                throw new ArgumentNullException("fragmentFlowService");
            }
            _FragmentFlowService = fragmentFlowService;
            if (fragmentTraversalV2 == null) {
                throw new ArgumentNullException("fragmentTraversalV2");
            }
            _FragmentTraversalV2 = fragmentTraversalV2;
            if (flowService == null) {
                throw new ArgumentNullException("flowService");
            }
            _FlowService = flowService;
            if (flowPropertyService == null) {
                throw new ArgumentNullException("flowPropertyService");
            }
            _FlowPropertyService = flowPropertyService;
            if (lciaMethodService == null) {
                throw new ArgumentNullException("lciaMethodService");
            }
            _LciaMethodService = lciaMethodService;
            if (processService == null) {
                throw new ArgumentNullException("processService");
            }
            _ProcessService = processService;
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
                IndicatorTypeID = TransformNullable(lm.IndicatorTypeID, "LCIAMethod.IndicatorTypeID"),
                Normalization = TransformNullable(lm.Normalization, "LCIAMethod.Normalization"),
                Weighting = TransformNullable(lm.Weighting, "LCIAMethod.Weighting"),
                UseAdvice = lm.UseAdvice,
                ReferenceFlowPropertyID = TransformNullable(lm.ReferenceQuantity, "LCIAMethod.ReferenceQuantity")
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

        public FlowResource Transform(Flow f) {
            return new FlowResource {
                FlowID = f.FlowID,
                Name = f.Name,
                FlowTypeID = TransformNullable(f.FlowTypeID, "Flow.FlowTypeID"),
                ReferenceFlowPropertyID = TransformNullable(f.ReferenceFlowProperty, "Flow.ReferenceFlowProperty"),
                CASNumber = f.CASNumber
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
                ReferenceUnitName = unitName
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
                 ProcessTypeID = TransformNullable(p.ProcessTypeID, "Process.ProcessTypeID"),
                 ReferenceTypeID = p.ReferenceTypeID,
                 ReferenceFlowID = p.ReferenceFlowID,
                 ReferenceYear = p.ReferenceYear
             };
         }

        // Get list methods 

        public IEnumerable<LCIAMethodResource> GetLCIAMethodResources() {
            IEnumerable<LCIAMethod> lciaMethods = _LciaMethodService.Query().Get();
            return lciaMethods.Select(lm => Transform(lm)).ToList();
        }

        /// <summary>
        /// Execute fragment traversal and return computation results in FragmentFlowResource objects
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <param name="scenarioID">ScenarioID filter for NodeCache</param>
        /// <returns>List of FragmentFlowResource objects</returns>
        public IEnumerable<FragmentFlowResource> GetFragmentFlowResources(int fragmentID, int scenarioID) {
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
        /// Get list of flows related to a fragment (via FragmentFlow)
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowResource objects</returns>
        public IEnumerable<FlowResource> GetFlowsByFragment(int fragmentID) {
            IEnumerable<Flow> flows = _FlowService.Query()
                                      .Filter(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentID))
                                      .Get();
            return flows.Select(f => Transform(f)).ToList();
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
        /// <returns>List of ProcessResource objects</returns>
        public IEnumerable<ProcessResource> GetProcesses() {
            IEnumerable<Process> pData = _ProcessService.Query().Get();
            return pData.Select(p => Transform(p)).ToList();
        }
    }
}
