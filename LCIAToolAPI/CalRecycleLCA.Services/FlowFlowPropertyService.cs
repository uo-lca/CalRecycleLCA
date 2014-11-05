using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalRecycleLCA.Repositories;

namespace CalRecycleLCA.Services
{
    public interface IFlowFlowPropertyService : IService<FlowFlowProperty>
    {
        double? FlowConv(int? myFlowId, int inFlowId, int scenarioId = 0);
    }

    public class FlowFlowPropertyService : Service<FlowFlowProperty>, IFlowFlowPropertyService
    {
        private readonly IRepositoryAsync<FlowFlowProperty> _repository;

        public FlowFlowPropertyService(IRepositoryAsync<FlowFlowProperty> repository)
            : base(repository)
        {
            _repository = repository;

        }
        /// <summary>
        /// Convert inflow into refFlow's reference property, based on inFlow's data.
        /// 
        /// ParamTypeID 4 applied here if scenarioID nonnull.  
        /// 
        /// </summary>
        /// <param name="refFlowId"></param>
        /// <param name="inFlowId"></param>
        /// <returns></returns>
        public double? FlowConv(int? refFlowId, int inFlowId, int scenarioId = 0)
        {
            if (refFlowId == null)
                return 1;
            else if (refFlowId == inFlowId)
                return 1;
            else
                return _repository.FlowConv((int)refFlowId, inFlowId, scenarioId);
        }
    }
}
