using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IFlowPropertyParamService : IService<FlowPropertyParam>
    {
    }

    public class FlowPropertyParamService : Service<FlowPropertyParam>, IFlowPropertyParamService
    {
        public FlowPropertyParamService(IRepositoryAsync<FlowPropertyParam> repository)
            : base(repository)
        {

        }
    }
}
