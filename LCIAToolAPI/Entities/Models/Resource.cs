using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Entities.Models
{
    public abstract class Resource 
    {
        public string UUID { get; set; }
        public string Version { get; set; }
        public List<Link> Links { get; set; }
        public string ResourceType { get; set; }
        [JsonIgnore]
        public virtual int ID { get; protected set; }
    }
}
