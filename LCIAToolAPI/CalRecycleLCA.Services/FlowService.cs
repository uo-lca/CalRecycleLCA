﻿using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using CalRecycleLCA.Repositories;

namespace CalRecycleLCA.Services
{
    public interface IFlowService : IService<Flow>
    {
        IEnumerable<FlowResource> GetFlowsByFragment(int fragmentId);
        IEnumerable<FlowResource> GetFlowsByLCIAMethod(int lciaMethodId);
        FlowResource GetFlow(int flowId);
        IEnumerable<FlowResource> GetFlows(int flowtypeID);
        IEnumerable<FlowResource> GetCompositionFlows();
    }

    public class FlowService : Service<Flow>, IFlowService
    {
        private IRepositoryAsync<Flow> _repository;

        public FlowService(IRepositoryAsync<Flow> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<FlowResource> GetFlows(int flowtypeID)
        {
            if (flowtypeID == 0)
                return _repository.GetFlows().ToList();
            else
                return _repository.GetFlows(flowtypeID).ToList();
        }

        public IEnumerable<FlowResource> GetCompositionFlows()
        {
            return _repository.GetCompositionFlows();
        }

        public FlowResource GetFlow(int flowId)
        {
            return _repository.GetFlow(flowId);
        }


        public IEnumerable<FlowResource> GetFlowsByFragment(int fragmentId)
        {
            return _repository.GetFlowsByFragment(fragmentId).ToList();
        }

        public IEnumerable<FlowResource> GetFlowsByLCIAMethod(int lciaMethodId)
        {
            return _repository.GetFlowsByLCIAMethod(lciaMethodId);
        }
    }
}
