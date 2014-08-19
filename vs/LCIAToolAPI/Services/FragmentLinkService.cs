using LcaDataModel;
using Entities.Models;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Services {
    /// <summary>
    /// FragmentLinkService - service for listing fragment links to be used in Sankey diagram
    /// Uses FragmentFlowService to query repository.
    /// </summary>
    public class FragmentLinkService : IFragmentLinkService {
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly IFragmentTraversalV2 _fragmentTraversalV2;

        public FragmentLinkService(IFragmentFlowService fragmentFlowService,
            IFragmentTraversalV2 fragmentTraversalV2) {
            if (fragmentFlowService == null) {
                throw new ArgumentNullException("fragmentFlowService is null");
            }
            _fragmentFlowService = fragmentFlowService;

            if (fragmentTraversalV2 == null)
            {
                throw new ArgumentNullException("fragmentTraversalV2 is null");
            }
            _fragmentTraversalV2 = fragmentTraversalV2;


        }

        private ICollection<LinkMagnitude> GetLinkMagnitudes(FragmentFlow ff, int scenarioID) {
            IEnumerable<FlowFlowProperty> ffpData = ff.Flow.FlowFlowProperties;
            IEnumerable<NodeCache> ncData = ff.NodeCaches;
            
            NodeCache nodeCache = ncData.Where(nc => nc.ScenarioID == scenarioID).FirstOrDefault();
            if (nodeCache == null) {
                return null;
            }
            else {
                double flowMagnitude = Convert.ToDouble(nodeCache.FlowMagnitude);
                return ffpData.Select(ffp =>
                        new LinkMagnitude {
                            FlowPropertyID = Convert.ToInt32(ffp.FlowPropertyID),
                            Magnitude = flowMagnitude * Convert.ToDouble(ffp.MeanValue)
                        }).ToList();
            }
            
        }

        private FragmentLink CreateFragmentLink(FragmentFlow ff, int scenarioID) {
            Debug.Assert(ff.NodeTypeID != null);
            Debug.Assert(ff.DirectionID != null);
            int? nullID = null;
            return new FragmentLink {
                FragmentFlowID = ff.FragmentFlowID,
                Name = ff.Name,
                ShortName = ff.ShortName,
                NodeTypeID = Convert.ToInt32(ff.NodeTypeID),
                DirectionID = Convert.ToInt32(ff.DirectionID),
                FlowID = ff.FlowID,
                ParentFragmentFlowID = ff.ParentFragmentFlowID,
                ProcessID = (ff.NodeTypeID == 1) ? ff.FragmentNodeProcesses.FirstOrDefault().FragmentNodeProcessID : nullID,
                SubFragmentID = (ff.NodeTypeID == 2) ? ff.FragmentNodeFragments.FirstOrDefault().FragmentNodeFragmentID : nullID,
                LinkMagnitudes = (ff.FlowID == null) ? null : GetLinkMagnitudes(ff, scenarioID)
            };
        }

        /// <summary>
        /// Get FragmentFlow data and transform to FragmentLink objects
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <param name="scenarioID">ScenarioID filter for NodeCache</param>
        /// <returns>List of FragmentLink objects</returns>
        public IEnumerable<FragmentLink> GetFragmentLinks(int fragmentID, int scenarioID) {
            _fragmentTraversalV2.Traverse(fragmentID, scenarioID);
            IEnumerable<FragmentFlow> ffData = _fragmentFlowService.GetFragmentFlows(fragmentID);
            return ffData.Select(ff => CreateFragmentLink(ff, scenarioID)).ToList();
        }
    }
}
