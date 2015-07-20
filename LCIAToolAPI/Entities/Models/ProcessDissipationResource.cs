using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    /// <summary>
    /// List of flow constituents (FlowProperties) that become converted into emissions by a dissipation process
    /// </summary>
    public class ProcessDissipationResource
    {
        public double DissipationFactor { get; set; }
        public int FlowPropertyID { get; set; }
        public double Scale { get; set; }
        public int EmissionFlowID { get; set; }
    }
}
