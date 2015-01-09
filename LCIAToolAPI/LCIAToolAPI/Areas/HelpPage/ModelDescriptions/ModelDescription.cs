using System;

namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Describes a type model for some entity. 
    /// </summary>
    public abstract class ModelDescription
    {
        /// <summary>
        /// The documentary content
        /// </summary>
        public string Documentation { get; set; }
        /// <summary>
        /// uh
        /// </summary>
        public Type ModelType { get; set; }
        /// <summary>
        /// the name ..? I must really hate compiler warnings.
        /// </summary>
        public string Name { get; set; }
    }
}