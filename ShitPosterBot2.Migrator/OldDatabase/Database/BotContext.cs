using Microsoft.EntityFrameworkCore;
using ShitPosterBotConsole.Database;

namespace ShitPosterBot.Database
{
    public class BotContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Statistics> Statistic { get; set; }

        public DbSet<Splash> Splashas { get; set; }

        public DbSet<TopSecret> TopSecrets { get; set; }

        public DbSet<Domain> Domains { get; set; }

        public DbSet<PostAttachment> Attachments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=old_botdata.db");
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
