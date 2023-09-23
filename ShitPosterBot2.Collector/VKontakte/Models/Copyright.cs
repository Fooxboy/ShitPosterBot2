using Newtonsoft.Json;

namespace ShitPosterBot2.Collector.VKontakte.Models;

public class Copyright
{
    [JsonProperty("link")]
    public string Link { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}