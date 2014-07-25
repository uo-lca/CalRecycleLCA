﻿using Data;
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
    class FragmentLinkService : IFragmentLinkService {
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;

        public FragmentLinkService(IFragmentFlowService fragmentFlowService) {
            if (fragmentFlowService == null) {
                throw new ArgumentNullException("fragmentFlowService is null");
            }
            _fragmentFlowService = fragmentFlowService;
        }

        private ICollection<LinkMagnitude> GetLinkMagnitudes(FragmentFlow ff, int scenarioID) {
            IEnumerable<FlowFlowProperty> ffpData = ff.Flow.FlowFlowProperties;
            IEnumerable<NodeCache> ncData = ff.NodeCaches;
            NodeCache nodeCache = ncData.Where(nc => nc.ScenarioID == scenarioID).FirstOrDefault();
            // TODO : use new NodeCache.FlowMagnitude property when available.
            double flowMagnitude = Convert.ToDouble(nodeCache.NodeWeight); 
            return ffpData.Select(ffp => 
                    new LinkMagnitude {
                        FlowPropertyID = ffp.FlowPropertyID,
                        FlowMagnitiude = flowMagnitude * Convert.ToDouble(ffp.MeanValue)
                }).ToList();
        }

        private FragmentLink CreateFragmentLink(FragmentFlow ff, int scenarioID) {
            Debug.Assert(ff.NodeTypeID != null);
            Debug.Assert(ff.DirectionID != null);
            Debug.Assert(ff.FlowID != null);
            int? nullID = null;
            return new FragmentLink {
                FragmentFlowID = ff.FragmentFlowID,
                Name = ff.Name,
                NodeTypeID = Convert.ToInt32(ff.NodeTypeID),
                DirectionID = Convert.ToInt32(ff.DirectionID),
                FlowID = Convert.ToInt32(ff.FlowID),
                ParentFragmentFlowID = ff.ParentFragmentFlowID,
                ProcessID = (ff.NodeTypeID == 1) ? ff.FragmentNodeProcesses.FirstOrDefault().FragmentNodeProcessID : nullID,
                SubFragmentID = (ff.NodeTypeID == 2) ? ff.FragmentNodeFragments.FirstOrDefault().FragmentNodeFragmentID : nullID,
                LinkMagnitudes = GetLinkMagnitudes(ff, scenarioID)
            };
        }

        public IEnumerable<FragmentLink> GetFragmentLinks(int fragmentID, int scenarioID) {
            IEnumerable<FragmentFlow> ffData = _fragmentFlowService.GetFragmentFlows(fragmentID);
            return ffData.Select(ff => CreateFragmentLink(ff, scenarioID));
        }
    }
}
