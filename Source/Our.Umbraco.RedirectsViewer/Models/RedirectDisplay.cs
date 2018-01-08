namespace Our.Umbraco.RedirectsViewer.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The redirect display model
    /// </summary>
    [DataContract(Name = "redirect", Namespace = "")]
    public class RedirectDisplay
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the old url.
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [DataMember(Name = "createDate")]
        public string CreateDate { get; set; }
    }
}
