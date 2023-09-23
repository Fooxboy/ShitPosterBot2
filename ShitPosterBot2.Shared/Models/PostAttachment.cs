using System.ComponentModel.DataAnnotations;

namespace ShitPosterBot2.Shared.Models;

public class PostAttachment
{
    [Key]
    public int Id { get; set; }
    
    public AttachmentType AttachmentType { get; set; }
    
    public string? Url { get; set; }
    
    public Post Post { get; set; }
    
}