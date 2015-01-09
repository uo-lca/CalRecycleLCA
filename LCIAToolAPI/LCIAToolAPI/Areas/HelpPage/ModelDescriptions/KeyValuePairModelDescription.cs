namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// key and value are semantically equal
    /// </summary>
    public class KeyValuePairModelDescription : ModelDescription
    {
        /// <summary>
        /// 
        /// </summary>
        public ModelDescription KeyModelDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ModelDescription ValueModelDescription { get; set; }
    }
}