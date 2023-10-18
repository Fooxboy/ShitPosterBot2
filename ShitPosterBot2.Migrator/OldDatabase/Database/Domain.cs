using System.ComponentModel.DataAnnotations;


namespace ShitPosterBotConsole.Database
{
    public class Domain
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Emoji { get; set; }

        public string? ContentType { get; set; }

        public bool? ShowOriginalText { get; set; }
    }
}
