using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Our.Umbraco.RedirectsViewer.Models
{
    [DataContract(Name = "RedirectSettings")]
    public class RedirectSettings
    {
        [DataMember(Name = "create")]
        public RedirectSetting Create { get; set; }

        [DataMember(Name = "delete")]
        public RedirectSetting Delete { get; set; }

    }

    [DataContract(Name = "RedirectSetting")]
    public class RedirectSetting
    {
        [DataMember(Name = "allowed")]
        public bool Allowed { get; set; }

        [DataMember(Name = "usergroups")]
        public List<string> UserGroups { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        public RedirectSetting(string key)
        {
            Key = key;
            UserGroups=new List<string>();
        }
    }
}
