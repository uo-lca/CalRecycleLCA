﻿using Data;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IProcessService : IService<Process>
    {
        IEnumerable<ProcessModel> GetProcesses();            
    }
}
