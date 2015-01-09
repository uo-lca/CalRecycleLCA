using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Enum type
    /// </summary>
    public class EnumTypeModelDescription : ModelDescription
    {
        /// <summary>
        /// constructor initializes members
        /// </summary>
        public EnumTypeModelDescription()
        {
            Values = new Collection<EnumValueDescription>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Collection<EnumValueDescription> Values { get; private set; }
    }
}