using System;

namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// ahh, a sealed class.
    /// Use this attribute to change the name of the <see cref="ModelDescription"/> generated for a type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class ModelNameAttribute : Attribute
    {
        /// <summary>
        /// the "name" attribute is sealed
        /// </summary>
        /// <param name="name">name</param>
        public ModelNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// the name
        /// </summary>
        public string Name { get; private set; }
    }
}