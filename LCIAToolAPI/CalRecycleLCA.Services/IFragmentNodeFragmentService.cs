﻿using Entities.Models;
using LcaDataModel;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface IFragmentNodeFragmentService : IService<FragmentNodeFragment>
    {
        FragmentNodeResource GetFragmentNodeSubFragmentId(int fragmentFlowId, int scenarioId = 0);
    }
}
