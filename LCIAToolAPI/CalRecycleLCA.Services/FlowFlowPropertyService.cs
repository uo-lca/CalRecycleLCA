using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalRecycleLCA.Repositories;
using Entities.Models;

namespace CalRecycleLCA.Services
{
    public interface IFlowFlowPropertyService : IService<FlowFlowProperty>
    {
        double? FlowConv(int? myFlowId, int inFlowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        //ICollection<FlowPropertyMagnitude> GetFlowPropertyMagnitudes(NodeCacheModel ff, int scenarioId);
        ICollection<FlowPropertyMagnitude> GetFlowPropertyMagnitudes(int flowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
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
        public double? FlowConv(int? refFlowId, int inFlowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            if ((refFlowId == null) || (refFlowId == inFlowId) || (refFlowId == 0))
                return 1;
            else
                return _repository.FlowConv((int)refFlowId, inFlowId, scenarioId);
        }

        public ICollection<FlowPropertyMagnitude> GetFlowPropertyMagnitudes(int flowId, int scenarioId = Scenario.MODEL_BASE_CASE_ID)
        {
            return _repository.GetFlowPropertyMagnitudes(flowId, scenarioId).ToList();
        }
        
        /*
        /// <summary>
        /// Takes in a FragmentFlowResource and returns scenario-specific flow magnitudes for the flow. 
        /// If the NodeWeight field is populated, the magnitudes are scaled by it.
        /// </summary>
        /// <param name="ncm"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public ICollection<FlowPropertyMagnitude> GetFlowPropertyMagnitudes(NodeCacheModel ncm, int scenarioId)
        {
            ICollection<FlowPropertyMagnitude> ffpData = 
                (ncm.FlowID == null ? new List<FlowPropertyMagnitude>()
                : _repository.GetFlowPropertyMagnitudes((int)ncm.FlowID, scenarioId)).ToList();
            if (ncm.FlowMagnitude != null)
                foreach (var fp in ffpData)
                {
                    fp.Magnitude *= (double)ncm.FlowMagnitude;
                }
            return ffpData;


        }
         * */
    }
}
