using LcaDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;

namespace CalRecycleLCA.Services
{
    public interface IDependencyParamService: IService<DependencyParam>
    {
        IEnumerable<DependencyParam> GetDependencyParams(int scenarioId = 1);

    }
}
