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
    public interface ICharacterizationParamService : IService<CharacterizationParam>
    {
    }

    public class CharacterizationParamService : Service<CharacterizationParam>, ICharacterizationParamService
    {
        public CharacterizationParamService(IRepositoryAsync<CharacterizationParam> repository)
            : base(repository)
        {
                    
        }
    }
}
