using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    
    public Domain DomainInfo { get; set; }
    
    public List<PostAttachment> Attachments { get; set; }
    
    [NotMapped]
    public int Tryes { get; set; }
}