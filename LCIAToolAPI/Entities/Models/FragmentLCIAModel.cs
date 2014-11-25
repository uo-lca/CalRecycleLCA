using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    /// <summary>
    /// This class is used strictly for returning aggregated LCIA results from 
    /// a collection of FragmentFlows.
    /// </summary>
    public class FragmentLCIAModel
    {
        public int? FragmentFlowID { get; set; }
        public int? FragmentStageID { get; set; }
        public int LCIAMethodID { get; set; }
        public double? NodeWeight { get; set; }
        public double? ImpactScore { get; set; }
	    public double Result { get; set; }

        //public ICollection<LCIAModel> NodeLCIAResults { get; set; } // not currently used
        //public int? fnpProcessID { get; set; }
        //public int? psProcessID { get; set; }
        //public int? fnpSubFragmentID { get; set; }
        //public int? psSubFragmentID { get; set; }
        //public int? NodeTypeID { get; set; }
        //public int? FlowID { get; set; }
        //public int? DirectionID { get; set; }
    }
}
