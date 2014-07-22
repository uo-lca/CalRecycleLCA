using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repository;
using Services;
using Data;
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

        int? GetNodeID(FragmentFlow ff) {
            switch (ff.NodeTypeID) {
                case 1: return ff.FragmentNodeProcesses.FirstOrDefault().FragmentNodeProcessID;
                case 2: return ff.FragmentNodeFragments.FirstOrDefault().FragmentNodeFragmentID;
            }
            return null;

        }

        [Route("api/fragments/{fragmentID}/fragmentflows")]
        [System.Web.Http.HttpGet]
        public IEnumerable<FragmentFlowModel> GetFragmentFlows(int fragmentID)
        
        {
            // TODO : Traversal of all fragments should be done at web api start up time.
            //        In the meantime, call api/fragments/{fragmentID}/scenarios/{scenarioID}/traverse
            //        before getting fragmentflows

            IEnumerable<FragmentFlow> ffData = _fragmentFlowService.GetFragmentFlows(fragmentID);
            return ffData.Select(ff => new FragmentFlowModel {
                fragmentFlowID = ff.FragmentFlowID,
                fragmentID = ff.FragmentID,
                fragmentStageID = ff.FragmentStageID,
                name = ff.Name,
                referenceFlowPropertyID = ff.ReferenceFlowPropertyID,
                nodeTypeID = ff.NodeTypeID,
                nodeWeight = (ff.NodeCaches.Count == 0) ? 0 : ff.NodeCaches.FirstOrDefault().NodeWeight,
                nodeID = GetNodeID(ff)
            });
        }
    }
}
