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
    public class FlowPropertyService : Service<FlowProperty>, IFlowPropertyService
    {
        private readonly IRepository<FlowProperty> _repository;

        public FlowPropertyService(IRepository<FlowProperty> repository)
            : base(repository)
        {
            _repository = repository;
        }

        private FlowPropertyModel TransformFlowProperty(FlowProperty fp) {
            string unitName = "";
            if (fp.UnitGroup != null && fp.UnitGroup.UnitConversion != null) {
                unitName = fp.UnitGroup.UnitConversion.Unit;
            }
            return new FlowPropertyModel {
                FlowPropertyID = fp.FlowPropertyID,
                Name = fp.Name,
                ReferenceUnitName = unitName
            };
        }

        /// <summary>
        /// Get FlowProperty data related to fragment and transform to API model
        /// </summary>
        /// <param name="fragmentID">FragmentID filter</param>
        /// <returns>List of FlowPropertyModel objects</returns>
        public IEnumerable<FlowPropertyModel> GetFlowPropertiesByFragment(int fragmentId) {
            IEnumerable<FlowProperty> fpData = _repository.GetFlowPropertiesByFragment(fragmentId);
            return fpData.Select(fp => TransformFlowProperty(fp)).ToList();
        }
    }
}
