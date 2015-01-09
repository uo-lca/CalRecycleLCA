namespace LCAToolAPI.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// An element of a collection -- extends ModelDescription
    /// </summary>
    public class CollectionModelDescription : ModelDescription
    {
        /// <summary>
        /// collection-specific supplemental doc
        /// </summary>
        public ModelDescription ElementDescription { get; set; }
    }
}