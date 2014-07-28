using LcaDataModel;
using Entities.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FlowService : Service<Flow>, IFlowService
    {
        private readonly IRepository<Flow> _repository;

        public FlowService(IRepository<Flow> repository)
            : base(repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get Flow data related to fragment and transform to API model
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowModel objects</returns>
        public IEnumerable<FlowModel> GetFlowsByFragment(int fragmentId) {
            IEnumerable<Flow> fData = _repository.GetFlowsByFragment(fragmentId);
            return fData.Select(f => new FlowModel {
                FlowID = f.FlowID,
                Name = f.Name,
                FlowTypeID = Convert.ToInt32(f.FlowTypeID),
                ReferenceFlowPropertyID = Convert.ToInt32(f.ReferenceFlowProperty),
                CASNumber = f.CASNumber
            }).ToList();
        }
    }
}
