using Newtonsoft.Json;

namespace ShitPosterBot2.Collector.VKontakte.Models;

public class PostResponse
{
    [JsonProperty("response")]
    public List<VkPost> Response { get; set; }
}