using Newtonsoft.Json;

namespace ShitPosterBot2.Collector.VKontakte.Models;

public class VkPost
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("copyright")]
    public Copyright? Copyright { get; set; }
}