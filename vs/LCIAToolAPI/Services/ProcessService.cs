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
    public class ProcessService : Service<Process>, IProcessService {
        private readonly IRepository<Process> _repository;

        public ProcessService(IRepository<Process> repository)
            : base(repository) {
            _repository = repository;
        }

        /// <summary>
        /// Get Process data and transform to API model
        /// </summary>
        /// <returns>List of ProcessModel objects</returns>
        public IEnumerable<ProcessModel> GetProcesses() {
            IEnumerable<Process> pData = _repository.GetProcesses();
            return pData.Select(p => new ProcessModel {
                ProcessID = p.ProcessID,
                Name = p.Name,
                ProcessTypeID = Convert.ToInt32(p.ProcessTypeID),
                ReferenceTypeID = p.ReferenceTypeID,
                ReferenceFlowID = p.ReferenceFlowID,
                ReferenceYear = p.ReferenceYear
            }).ToList();
        }
    }
}
