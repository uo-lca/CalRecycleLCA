﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services {
    interface IFragmentLinkService {
        IEnumerable<FragmentLink> GetFragmentLinks(int fragmentId, int scenarioId);
    }
}
