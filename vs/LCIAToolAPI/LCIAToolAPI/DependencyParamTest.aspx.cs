using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using Ninject;

namespace LCAToolAPI
{
    public partial class DependencyParamTest : Ninject.Web.PageBase
    {
        [Inject]
        public IFragmentTraversal Model { get; set; }
        

        protected void Page_Load(object sender, EventArgs e)
        {
            depTest.DataSource = Model.ApplyDependencyParam(0);
            depTest.DataBind();
        }
    }
}