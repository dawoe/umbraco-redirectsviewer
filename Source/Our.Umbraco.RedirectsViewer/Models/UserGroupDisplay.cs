namespace Our.Umbraco.RedirectsViewer.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Usergroup display model
    /// </summary>
    [DataContract(Name = "usergroup")]
    internal class UserGroupDisplay
    {
        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
