using Ninject;
using CalRecycleLCA.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LCAToolAPI
{
    public partial class Timer : System.Web.UI.Page
    {
        [Inject]
        public IFragmentTraversalV2 Model { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Model.Traverse(11, 1);
            sw.Stop();
            Label1.Text = sw.Elapsed.Seconds.ToString();
        }
    }
}