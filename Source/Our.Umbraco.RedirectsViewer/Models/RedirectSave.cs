namespace Our.Umbraco.RedirectsViewer.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The redirect save.
    /// </summary>
    [DataContract(Name = "redirect")]
    public class RedirectSave
    {
        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the content key.
        /// </summary>
        [DataMember(Name = "contentKey")]
        public Guid ContentKey { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        [DataMember(Name = "culture")]
        public string Culture { get; set; }
    }
}
