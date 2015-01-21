using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ApiInfo
    {
        public ApiInfo()
        {
            Links = new List<Link>();
        }
        public string Title { get; set; }
        public string Engine { get { return "Antelope LCA (.NET)"; } }
        public string Copyright 
        { 
            get 
            { return "(2015) California Department of Resources Recycling and Recovery (CalRecycle)"; } 
        }
        public string Version { get; set; }

        public Contact Maintainer { get; set; }

        public List<Link> Links;

    }
}
