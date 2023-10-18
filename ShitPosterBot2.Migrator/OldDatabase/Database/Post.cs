using ShitPosterBotConsole.Database;
using System.ComponentModel.DataAnnotations;

namespace ShitPosterBot.Database
{
    public class Post
    {
        [Key]
        public long Id { get; set; }

        public long? VkId { get; set; }

        public string? Domain { get; set; }

        public DateTime? CreatedTime { get; set; }

        public string? Text { get; set; }

        public string? OriginalText { get; set; }

        public List<PostAttachment>? Attachments { get; set; }
    }
}
