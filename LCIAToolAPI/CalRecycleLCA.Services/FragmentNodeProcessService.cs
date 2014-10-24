using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalRecycleLCA.Repositories;
using Entities.Models;


namespace CalRecycleLCA.Services
{

    public class FragmentNodeProcessService : Service<FragmentNodeProcess>, IFragmentNodeProcessService
    {
        private readonly IRepositoryAsync<FragmentNodeProcess> _repository;
        public FragmentNodeProcessService(IRepositoryAsync<FragmentNodeProcess> repository)
            : base(repository)
        {
            _repository = repository;   
        }

        //Gets the processId from the FragmentNodeFragment table
        //Needs to be expanded to get this from processsubstitution table if present
        public FragmentNodeResource GetFragmentNodeProcessId(int fragmentFlowId)
        {
            return _repository.GetFragmentNodeProcessId(fragmentFlowId);

        }
    }
}
