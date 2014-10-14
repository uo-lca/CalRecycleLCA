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
    public interface IFlowPropertyEmissionService : IService<FlowPropertyEmission>
    {
    }

    public class FlowPropertyEmissionService : Service<FlowPropertyEmission>, IFlowPropertyEmissionService
    {
        public FlowPropertyEmissionService(IRepositoryAsync<FlowPropertyEmission> repository)
            : base(repository)
        {
                    
        }
    }
}
