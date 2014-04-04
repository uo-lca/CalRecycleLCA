using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LCIATool.Models.Repository;

namespace LCIATool.Pages
{
    public partial class LCIAComputation : System.Web.UI.Page
    {
        private LCAToolDevEntities2 context = new LCAToolDevEntities2();
        protected void Page_Load(object sender, EventArgs e)
        {
            var _lciaList = (from p in context.Processes
                             join pf in context.ProcessFlows on p.ProcessID equals pf.ProcessFlowProcessID
                             join f in context.Flows on pf.ProcessFlowFlowID equals f.FlowID
                             join l in context.LCIAs on f.FlowID equals l.LCIAFlowID
                             join lm in context.LCIAMethods on l.LCIAMethodID equals lm.LCIAMethodID
                             join fp in context.FlowProperties on f.FlowPropertyID equals fp.FlowPropertyID
                             where f.FlowTypeID == 2
                             select new
                             {
                                 pf.Result,
                                 l.Factor,
                                 LCIAResult = pf.Result * l.Factor
                             });
            gvLCIAComp.DataSource = _lciaList.ToList();
            gvLCIAComp.DataBind();

            //var _lciaList = (from p in context.Processes
            //                 where p.ProcessID == p.ProcessFlows.
            //                 select new PersonInfo
            //                 {
            //                     Name = p.FirstName + " " + p.LastName,
            //                     BornIn = p.BornInCity.Name,
            //                     LivesIn = p.LivesInCity.Name,
            //                     Gender = p.Sex.Name,
            //                     CarsOwnedCount = p.Cars.Count(),
            //                 });

        }
    }
}