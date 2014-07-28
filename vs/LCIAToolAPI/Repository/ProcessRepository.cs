using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Repository
{
    public static class ProcessRepository {
        /// <summary>
        /// Query for all processses
        /// </summary>
        /// <returns>query result</returns>
        public static IEnumerable<Process> GetProcesses(this IRepository<Process> processRepository) {
            return processRepository.GetRepository<Process>().Query().Get();
        }

    }
}
