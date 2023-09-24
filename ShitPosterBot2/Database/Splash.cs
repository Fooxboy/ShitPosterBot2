using System.ComponentModel.DataAnnotations;

namespace ShitPosterBot2.Database;

public class Splash
{
    [Key]
    public long Id { get; set; }

    public string? Message { get; set; }

}