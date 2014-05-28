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
        int proc=0;
        int lciameth = 0;
        private LCAToolDevEntities1 context = new LCAToolDevEntities1();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var process = (from p in context.Processes
                               join pf in context.ProcessFlows on p.ProcessID equals pf.ProcessID
                               join f in context.Flows on pf.FlowID equals f.FlowID
                               join l in context.LCIAs on f.FlowID equals l.FlowID
                               join lm in context.LCIAMethods on l.LCIAMethodID equals lm.LCIAMethodID
                               join ft in context.FlowTypes on f.FlowTypeID equals ft.FlowTypeID
                               join fp in context.FlowProperties on f.FlowPropertyID equals fp.FlowPropertyID
                               where f.FlowTypeID == 2
                               select new
                               {
                                   Process = p.ProcessType,
                                   ProcessID = p.ProcessID,
                               }).ToList()
                               .GroupBy(proc => proc.Process)
                               .Select(proc => proc.First());
                ddlProcess.DataSource = process;
                ddlProcess.DataTextField = "Process";
                ddlProcess.DataValueField = "ProcessID";
                ddlProcess.DataBind();
                ddlProcess.Items.Insert(0, new ListItem("Select", "0"));

                var lciaMethod = (from p in context.Processes
                               join pf in context.ProcessFlows on p.ProcessID equals pf.ProcessID
                               join f in context.Flows on pf.FlowID equals f.FlowID
                               join l in context.LCIAs on f.FlowID equals l.FlowID
                               join lm in context.LCIAMethods on l.LCIAMethodID equals lm.LCIAMethodID
                               join ft in context.FlowTypes on f.FlowTypeID equals ft.FlowTypeID
                               join fp in context.FlowProperties on f.FlowPropertyID equals fp.FlowPropertyID
                               where f.FlowTypeID == 2
                               select new
                               {
                                   LCIAMethod = lm.Name,
                                   LCIAMethodID = lm.LCIAMethodID,
                               }).ToList()
                              .GroupBy(lciam => lciam)
                              .Select(lciam => lciam.First());
                ddlLCIAMethod.DataSource = lciaMethod;
                ddlLCIAMethod.DataTextField = "LCIAMethod";
                ddlLCIAMethod.DataValueField = "LCIAMethodID";
                ddlLCIAMethod.DataBind();
                ddlLCIAMethod.Items.Insert(0, new ListItem("Select", "0"));

            }

            bindGV();
        }

        protected void ddlProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindGV();
        }

        protected void bindGV()
        {
            proc = int.Parse(ddlProcess.SelectedValue);
            lciameth = int.Parse(ddlLCIAMethod.SelectedValue);

            var _lciaList = (from p in context.Processes
                             join pf in context.ProcessFlows on p.ProcessID equals pf.ProcessID
                             join f in context.Flows on pf.FlowID equals f.FlowID
                             join l in context.LCIAs on f.FlowID equals l.FlowID
                             join lm in context.LCIAMethods on l.LCIAMethodID equals lm.LCIAMethodID
                             join ft in context.FlowTypes on f.FlowTypeID equals ft.FlowTypeID
                             join fp in context.FlowProperties on f.FlowPropertyID equals fp.FlowPropertyID
                             where f.FlowTypeID == 2 && (p.ProcessID == proc || proc == 0) && (lm.LCIAMethodID == lciameth || lciameth == 0)
                             group new { p, pf, f, l, lm, ft } by new
                             {
                                 p.Name,
                                 lciamethodname= lm.Name,
                                 FlowType = ft.Name,
                                 pf.Result,
                                 p.Geography,
                                 l.Factor
                             } into g
                             select new
                             {
                                 Process = g.Key.Name,
                                 LCIAMethod = g.Key.lciamethodname,
                                 FlowType = g.Key.FlowType,
                                 Result = g.Key.Result,
                                 Geography = g.Key.Geography,
                                 Factor = g.Key.Factor,
                                 LCIAResult = g.Key.Result * g.Key.Factor
                             });
            gvLCIAComp.DataSource = _lciaList.ToList();
            gvLCIAComp.DataBind();
        }

        protected void ddlLCIAMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindGV();
        }

        //var _lciaList = (from p in context.Processes
        //                 join pf in context.ProcessFlows on p.ProcessID equals pf.ProcessFlowProcessID
        //                 join f in context.Flows on pf.ProcessFlowFlowID equals f.FlowID
        //                 join l in context.LCIAs on f.FlowID equals l.LCIAFlowID
        //                 join lm in context.LCIAMethods on l.LCIAMethodID equals lm.LCIAMethodID
        //                 join ft in context.FlowTypes on f.FlowTypeID equals ft.FlowTypeID
        //                 join fp in context.FlowProperties on f.FlowPropertyID equals fp.FlowPropertyID
        //                 where f.FlowTypeID == 2
        //                 select new
        //                 {
        //                     //p.ProcessUUID,
        //                     //f.FlowUUID,
        //                     //lm.LCIAMethodUUID,
        //                     p.ProcessType,
        //                     LCIAMethod = lm.LCIAMethod1,
        //                     FlowType = ft.FlowType1,
        //                     pf.Result,
        //                     l.Factor,
        //                     LCIAResult = pf.Result * l.Factor
        //                 });
        //gvLCIAComp.DataSource = _lciaList.ToList();
        //gvLCIAComp.DataBind();
    }
}