using System;
using System.Reflection;

namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Ah- the interface- now I get it
    /// i.e. read this first
    /// </summary>
    public interface IModelDocumentationProvider
    {
        /// <summary>
        /// I've got a list of members and I'll give you documentation for one of them.
        /// </summary>
        /// <param name="member">the guy</param>
        /// <returns>string</returns>
        string GetDocumentation(MemberInfo member);

        /// <summary>
        /// oh, I can also give you documentation on types of members.
        /// </summary>
        /// <param name="type">the type</param>
        /// <returns>string</returns>
        string GetDocumentation(Type type);
    }
}