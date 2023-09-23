using System.ComponentModel.DataAnnotations;

namespace ShitPosterBot2.Shared.Models;

public class Post
{
    [Key]
    public long Id { get; set; }
    
    public string PlatformId { get; set; }
    
    public string PlatformOwner { get; set; }
    
    public DateTime? CollectedAt { get; set; }
    
    public DateTime? PublishAt { get; set; }
    
    public string? Body { get; set; }
    
    public List<PostAttachment> Attachments { get; set; }
}