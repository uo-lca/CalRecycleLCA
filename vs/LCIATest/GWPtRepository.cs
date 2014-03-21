using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace LCIATest
{
    public class GWPtRepository : IGWPtRepository
    {
        private List<GWPt> GWPtList = new List<GWPt>();
            public GWPtRepository()
        {
            GWPtList.Add(new GWPt { Scenario = "2010 Base Year", CollectAndWaste = "35945437.59", Reprocessing = "56887872.41", Use = "512813574.5", DispUse = float.Parse("-507765022.9", CultureInfo.InvariantCulture.NumberFormat), DispProd = "-185352612.7" });
            GWPtList.Add(new GWPt { Scenario = "ReRe", CollectAndWaste = "33164483.71", Reprocessing = "105282168.8", Use = "75957482.38", DispUse = float.Parse("-76401504.77", CultureInfo.InvariantCulture.NumberFormat), DispProd = "-30872620" });
            GWPtList.Add(new GWPt { Scenario = "MDO", CollectAndWaste = "33164483.71", Reprocessing = "49654623.73", Use = "580662627.9", DispUse = float.Parse("-580795905.1", CultureInfo.InvariantCulture.NumberFormat), DispProd = "-143718257.3" });
            GWPtList.Add(new GWPt { Scenario = "RFO", CollectAndWaste = "33164483.71", Reprocessing = "1387904.628", Use = "971982747.9", DispUse = float.Parse("-929293149.7", CultureInfo.InvariantCulture.NumberFormat), DispProd = "-204159991.3" });
        }
        public IEnumerable<GWPt> GetAllData()
        {
            return GWPtList;
        }
    }
}