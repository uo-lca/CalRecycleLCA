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
    public interface IFlowTypeService : IService<FlowType>
    {
    }

    public class FlowTypeService : Service<FlowType>, IFlowTypeService
    {
        public FlowTypeService(IRepositoryAsync<FlowType> repository)
            : base(repository)
        {

        }
    }
}

