using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ParamInstance
    {
        public int? ParamTypeID; // could be omitted- should be obvious from context
        public int ParamID;
        public double Value;

    }
}
