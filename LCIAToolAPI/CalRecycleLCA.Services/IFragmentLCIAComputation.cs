﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;

namespace CalRecycleLCA.Services
{
    public interface IFragmentLCIAComputation
    {
        IEnumerable<NodeCache> FragmentTraverse(int fragmentId, int scenarioId = Scenario.MODEL_BASE_CASE_ID);
        IEnumerable<ScoreCache> FragmentLCIAComputeNoSave(int fragmentId, int scenarioId);
        void FragmentLCIAComputeSave(int fragmentId, int scenarioId);
        //void FragmentFlowLCIA(int? fragmentId, int scenarioId, IEnumerable<int> lciaMethods);
        IEnumerable<FragmentLCIAModel> FragmentLCIA(int? fragmentId, int? scenarioId, int? lciaMethodId);
    }
}
