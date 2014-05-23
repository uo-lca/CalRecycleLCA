using System;
using System.Collections.Generic;

namespace IlcdDataLoader.Models
{
    public partial class Source
    {
        public int SourceID { get; set; }
        public string SourceUUID { get; set; }
        public string SourceVersion { get; set; }
        public string Source1 { get; set; }
        public string Citation { get; set; }
        public string PubType { get; set; }
        public string URI { get; set; }
    }
}
