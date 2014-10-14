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
    public interface IProcessSubstitutionService : IService<ProcessSubstitution>
    {
    }

    public class ProcessSubstitutionService : Service<ProcessSubstitution>, IProcessSubstitutionService
    {
        public ProcessSubstitutionService(IRepositoryAsync<ProcessSubstitution> repository)
            : base(repository)
        {

        }
    }



}
