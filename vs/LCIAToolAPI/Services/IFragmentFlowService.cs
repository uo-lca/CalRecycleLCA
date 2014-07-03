﻿using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IFragmentFlowService : IService<FragmentFlow>
    {
        IEnumerable<FragmentFlow> GetFragmentFlows(int fragmentId);
    }
}