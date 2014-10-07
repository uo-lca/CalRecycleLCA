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
   

    public class FlowService : Service<Flow>, IFlowService
    {
        public FlowService(IRepositoryAsync<Flow> repository)
            : base(repository)
        {

        }
    }
}
