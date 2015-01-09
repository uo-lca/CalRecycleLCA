using System.Collections.ObjectModel;

namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// A complex type is an entity with many properties.  Extends <see cref="ModelDescription"/>
    /// </summary>
    public class ComplexTypeModelDescription : ModelDescription
    {
        /// <summary>
        /// constructor initializes members
        /// </summary>
        public ComplexTypeModelDescription()
        {
            Properties = new Collection<ParameterDescription>();
        }

        /// <summary>
        /// the properties
        /// </summary>
        public Collection<ParameterDescription> Properties { get; private set; }
    }
}