﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// FragmentResource - Used for producing fragment web service data.
    /// Simplifies EF model by omitting navigation properties and ILCDEntityID
    ///
    /// Maintains Pascal case of properties in Data model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    public class FragmentResource : Resource 
    {
        public FragmentResource() : base("Fragment") { }

        public override int ID { get { return FragmentID; } }

        public int FragmentID { get; set; }

        public int  ReferenceFragmentFlowID { get; set; }

        public int TermFlowID { get; set; }
        public string Direction { get; set; }

    }
}
