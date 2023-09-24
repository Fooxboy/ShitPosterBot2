using System.ComponentModel.DataAnnotations;

namespace ShitPosterBot2.Database;

public class TopSecret
{
    [Key]
    public string Key { get; set; }
    
    public string Value { get; set; }
}