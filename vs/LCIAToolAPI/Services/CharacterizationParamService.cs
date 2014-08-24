using LcaDataModel;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CharacterizationParamService : Service<CharacterizationParam>, ICharacterizationParamService
    {
        [Inject]
        private readonly IRepository<CharacterizationParam> _repository;


        public CharacterizationParamService(IRepository<CharacterizationParam> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
