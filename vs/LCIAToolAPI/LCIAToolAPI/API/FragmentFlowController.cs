using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repository;
using Services;
using LcaDataModel;
using Entities.Models;
using System.Web.Http.Services;
using Ninject;
using LCAToolAPI.Infrastructure;
using Ninject.Extensions.MissingBindingLogger;
using log4net;
using System.Reflection;
using Ninject.Extensions.Logging;

namespace LCAToolAPI.API
{
    // TODO : delete this class after changing Visualization to use FragmentLinkController

    public class FragmentFlowController : ApiController
    {
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;

        public FragmentFlowController(IFragmentFlowService fragmentFlowService)
        {

            if (fragmentFlowService == null)
            {
                throw new ArgumentNullException("fragmentFlowService is null");
            }

            _fragmentFlowService = fragmentFlowService;


        }

        [Route("api/fragments/{fragmentID}/fragmentflows")]
        [System.Web.Http.HttpGet]
        public IEnumerable<FragmentFlowModel> GetFragmentFlows(int fragmentID)  {
            // TODO : Traversal of all fragments should be done at web api start up time.
            //        In the meantime, call api/fragments/{fragmentID}/scenarios/{scenarioID}/traverse
            //        before getting fragmentflows

            IEnumerable<FragmentFlow> ffData = _fragmentFlowService.GetFragmentFlows(fragmentID);
            int? nullID = null;
            return ffData.Select(ff => new FragmentFlowModel {
                FragmentFlowID = ff.FragmentFlowID,
                FragmentID = ff.FragmentID,
                FragmentStageID = ff.FragmentStageID,
                Name = ff.Name,
                ReferenceFlowPropertyID = ff.ReferenceFlowPropertyID,
                NodeTypeID = ff.NodeTypeID,
                DirectionID = ff.DirectionID,
                FlowID = ff.FlowID,
                ParentFragmentFlowID = ff.ParentFragmentFlowID,
                // TODO : All fragment flows should have a node cache according to BK. 
                // If that is true, then Traverse has a bug. Currently, not all fragment flows have a node cache after traversal.
                ScenarioID = (ff.NodeCaches.Count == 0) ? nullID : ff.NodeCaches.FirstOrDefault().ScenarioID,
                NodeWeight = (ff.NodeCaches.Count == 0) ? null : ff.NodeCaches.FirstOrDefault().NodeWeight,  
                ProcessID = (ff.NodeTypeID == 1) ? ff.FragmentNodeProcesses.FirstOrDefault().FragmentNodeProcessID : nullID,
                SubFragmentID = (ff.NodeTypeID == 2) ? ff.FragmentNodeFragments.FirstOrDefault().FragmentNodeFragmentID : nullID
            });
        }
    }
}
