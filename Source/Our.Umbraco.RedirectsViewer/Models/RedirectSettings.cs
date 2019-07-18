using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Our.Umbraco.RedirectsViewer.Models
{
    [DataContract(Name = "RedirectSettings")]
    public class RedirectSettings
    {
        [DataMember(Name = "allowed")]
        public bool Allowed { get; set; }

        [DataMember(Name = "usergroups")]
        public List<string> UserGroups { get; set; }
    }
}
