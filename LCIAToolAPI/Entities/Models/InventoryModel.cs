using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class InventoryModel
    {
        public int FlowID { get; set; }

        public int DirectionID { get; set; }

        public double? Result { get; set; }

        public ParamInstance Param { get; set; }
    }
}
