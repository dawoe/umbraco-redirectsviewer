using Newtonsoft.Json;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.RedirectsViewer.Models.Import
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RedirectItem
    {
        
        public IPublishedContent Content;
        [JsonProperty]
        public string Url;

        public string Target;

    }
}