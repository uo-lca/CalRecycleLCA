using Entities.Models;
using LcaDataModel;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IFragmentNodeProcessService : IService<FragmentNodeProcess>
    {
        FragmentNodeResource GetFragmentNodeProcessId(int fragmentFlowId, int scenarioID = 0);
    }
}
