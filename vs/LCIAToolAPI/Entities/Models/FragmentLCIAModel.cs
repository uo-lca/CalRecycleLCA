using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class FragmentLCIAModel
    {
        public int? FragmentFlowID { get; set; }
        public double? NodeWeight { get; set; }
        public double? ImpactScore { get; set; }
        public int? fnpProcessID { get; set; }
        public int? psProcessID { get; set; }
        public int? fnpSubFragmentID { get; set; }
        public int? psSubFragmentID { get; set; }
        public int? NodeTypeID { get; set; }
        public int? FlowID { get; set; }
        public int? DirectionID { get; set; }
    }
}
