using Ninject;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LCAToolAPI
{
    public partial class FlowPropertyParamTest : Ninject.Web.PageBase
    {
        [Inject]
        public IFragmentTraversal Model { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            fpTest.DataSource = Model.ApplyFlowPropertyParam(0);
            fpTest.DataBind();
        }
    }
}