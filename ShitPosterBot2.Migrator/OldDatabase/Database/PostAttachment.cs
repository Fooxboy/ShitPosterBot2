using System.ComponentModel.DataAnnotations;
using ShitPosterBot2.Migrator.OldDatabase.Database;

namespace ShitPosterBotConsole.Database
{
    public class PostAttachment
    {
        [Key]
        public int Id { get; set; }

        public string? Uri { get; set; }

        public ContentType? ContentType { get; set; }

        public long? ParentPostId { get; set; }
    }
}
