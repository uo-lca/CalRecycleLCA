using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Entities.Models
{
    public class Resource
    {
        public Resource(string resourceType)
        {
            Links = new List<Link>();
            ResourceType = resourceType;
        }
        public string Name { get; set; }

        public string UUID { get; set; }
        public string Version { get; set; }
        public List<Link> Links { get; set; }
        public string ResourceType { get; private set; }
        public bool? isPrivate { get; set; }
        [JsonIgnore]
        public virtual int ID { get; protected set; }
    }
}
