using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models {
    /// <summary>
    /// Used for producing web API Impact Category resources.
    /// Simplifies EF model by omitting navigation properties and ILCDEntityID
    /// Maintains Pascal case of properties in EF model. These are automatically converted to
    /// camel case during JSON serialization.
    /// </summary>
    /// 
    public class ImpactCategoryResource {

        public int ImpactCategoryID { get; set; }
        public string Name { get; set; }
    }

}
