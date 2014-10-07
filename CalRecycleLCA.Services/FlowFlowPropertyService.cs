﻿using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IFlowFlowPropertyService : IService<FlowFlowProperty>
    {
    }

    public class FlowFlowPropertyService : Service<FlowFlowProperty>, IFlowFlowPropertyService
    {
        public FlowFlowPropertyService(IRepositoryAsync<FlowFlowProperty> repository)
            : base(repository)
        {
                    
        }
    }
}
