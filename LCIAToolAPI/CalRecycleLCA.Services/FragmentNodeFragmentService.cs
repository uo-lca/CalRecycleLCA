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
using Entities;

namespace CalRecycleLCA.Services
{

    public class FragmentNodeFragmentService : Service<FragmentNodeFragment>, IFragmentNodeFragmentService
    {
        private readonly IRepositoryAsync<FragmentNodeFragment> _repository;

        public FragmentNodeFragmentService(IRepositoryAsync<FragmentNodeFragment> repository)
            : base(repository)
        {
            _repository = repository;
        }


        //Gets the subfragmentId from the FragmentNodeFragment table
        //Needs to be expanded to get this from processsubstitution table if present
        public FragmentNodeResource GetFragmentNodeSubFragmentId(int fragmentFlowId)
        {
            return _repository.GetFragmentNodeSubFragmentId(fragmentFlowId);

        }
    }
}
