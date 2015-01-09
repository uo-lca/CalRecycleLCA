using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// A parameter attaches a set of annotations to a type model.
    /// </summary>
    public class ParameterDescription
    {
        /// <summary>
        /// constructor initializes members
        /// </summary>
        public ParameterDescription()
        {
            Annotations = new Collection<ParameterAnnotation>();
        }

        /// <summary>
        /// The annotations--
        /// </summary>
        public Collection<ParameterAnnotation> Annotations { get; private set; }

        /// <summary>
        /// Documentation of the parameter
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        ///  param name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// What is being annotated
        /// </summary>
        public ModelDescription TypeDescription { get; set; }
    }
}