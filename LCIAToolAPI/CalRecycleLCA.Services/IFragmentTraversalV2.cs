﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;

namespace CalRecycleLCA.Services
{
    //Version 2 of fragment traversal - rewritten to reflect the pseudocode dated Mon Jul 28 00:32:01 -0700 2014
    public interface IFragmentTraversalV2
    {
        IEnumerable<NodeCacheModel> EnterTraversal(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<FragmentFlowResource> SensitivityTraverse(FragmentFlowResource ffr, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
    }
}
