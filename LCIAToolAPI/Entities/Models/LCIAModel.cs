using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class LCIAModel
    {
        public int ScenarioID { get; set; }
        public int LCIAMethodID { get; set; }
        public int FlowID { get; set; }
        public int DirectionID { get; set; }
        public double? Composition { get; set; }
        public double? Dissipation { get; set; }
        public double Quantity { get; set; }
        public double Factor { get; set; }
        public double Result { get; set; }
        public ParamInstance CharacterizationParam { get; set; }
        public string Geography { get; set; }
    }

    /*
    public class LCIAResult
    {
        public LCIAResult()
        {
            LCIADetail = new List<LCIAModel> () ;
        }
        public int? ScenarioID { get; set; }
        public int LCIAMethodID { get; set; }
        public double Total { get { return LCIADetail.Sum(k => k.Result); } }
        public ICollection<LCIAModel> LCIADetail { get; set; }
    }
     * */
}
