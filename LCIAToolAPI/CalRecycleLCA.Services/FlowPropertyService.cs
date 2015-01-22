using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using CalRecycleLCA.Repositories;

namespace CalRecycleLCA.Services
{
    public interface IFlowPropertyService : IService<FlowProperty>
    {
        FlowPropertyResource GetResource(int flowPropertyId);
        List<FlowPropertyResource> GetAllResources();
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processID);
        IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentId);
    }

    public class FlowPropertyService : Service<FlowProperty>, IFlowPropertyService
    {
        private IRepositoryAsync<FlowProperty> _repository;

        public FlowPropertyService(IRepositoryAsync<FlowProperty> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public FlowPropertyResource GetResource(int flowPropertyId)
        {
            return _repository.GetResources()
                .Where(fp => fp.FlowPropertyID == flowPropertyId).FirstOrDefault();
        }

        public List<FlowPropertyResource> GetAllResources()
        {
            return _repository.GetResources().ToList();
        }

        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(int processId)
        {
            return _repository.GetFlowPropertiesByProcess(processId);
        }

        public IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(int fragmentId)
        {
            return _repository.GetFlowPropertiesByFragment(fragmentId);
        }
    }
}
