﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Repository
{
    public static class FlowRepository
    {
        public static IEnumerable<Flow> GetFlows(this IRepository<Flow> flowRepository) {
            return flowRepository.GetRepository<Flow>().Queryable();
        }
    }
}
