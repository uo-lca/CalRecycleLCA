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
    public interface IFlowPropertyService : IService<FlowProperty>
    {
    }

    public class FlowPropertyService : Service<FlowProperty>, IFlowPropertyService
    {
        public FlowPropertyService(IRepositoryAsync<FlowProperty> repository)
            : base(repository)
        {

        }
    }
}
