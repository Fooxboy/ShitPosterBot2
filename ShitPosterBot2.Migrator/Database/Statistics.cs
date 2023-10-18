using System.ComponentModel.DataAnnotations;

namespace ShitPosterBot2.Database;

public class Statistics
{
    [Key]
    public long Id { get; set; }
    
    public StatisticType StatType { get; set; }
    
    public string Argument { get; set; }
}