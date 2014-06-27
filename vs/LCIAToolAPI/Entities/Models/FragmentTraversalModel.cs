using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class FragmentTraversalModel
    {
        public int FragmentID { get; set; }

        public string UUID { get; set; }

        public string Name { get; set; }

        public int? ReferenceFragmentFlowID { get; set; }
    }
}
