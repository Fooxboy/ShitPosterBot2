using Microsoft.EntityFrameworkCore;
using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Database;

public class BotContext : DbContext
{

    public DbSet<Post> Posts { get; set; }

    public DbSet<PostAttachment> Attachments { get; set; }
    
    public DbSet<TopSecret> TopSecrets { get; set; }
    
    public DbSet<Domain> Domains { get; set; }
    
    public DbSet<Statistics> Statistics { get; set; }
    
    public DbSet<Splash> Splashes { get; set; }
    
    public BotContext(DbContextOptions<BotContext> options) : base(options)
    {
        
    }
}