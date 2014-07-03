﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Repository
{
    public static class FragmentFlowRepository
    {
        public static IEnumerable<FragmentFlow> GetFragmentFlows(this IRepository<FragmentFlow> fragmentFlowRepository, int fragmentId)
        {
            var fragmentFlows =
       fragmentFlowRepository
            .Query()
            .OrderBy(q => q
                .OrderBy(c => c.FlowID)
                .ThenBy(c => c.Name))
            .Filter(q => q.FragmentID == fragmentId)
            .GetPage();

            return fragmentFlows;
        }
    }
}