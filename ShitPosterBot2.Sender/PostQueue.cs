using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Sender;

public class PostQueue
{
    public Post Post { get; set; }
    
    public string Target { get; set; }
}