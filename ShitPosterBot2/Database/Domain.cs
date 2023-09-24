using System.ComponentModel.DataAnnotations;

namespace ShitPosterBot2.Database;

public class Domain
{
    [Key]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Emoji { get; set; }
    
    public bool ShowOriginalText { get; set; }
    
    public string? Target { get; set; }
}