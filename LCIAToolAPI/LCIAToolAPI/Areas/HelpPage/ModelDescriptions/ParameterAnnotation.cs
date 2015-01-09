using System;

namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// An annotation has an attribute.
    /// </summary>
    public class ParameterAnnotation
    {
        /// <summary>
        /// the attribute
        /// </summary>
        public Attribute AnnotationAttribute { get; set; }
        /// <summary>
        /// how the attribute came to be known, e.g.
        /// </summary>
        public string Documentation { get; set; }
    }
}