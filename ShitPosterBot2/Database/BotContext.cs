using Microsoft.EntityFrameworkCore;

namespace ShitPosterBot2.Database;

public class BotContext : DbContext
{
    public BotContext(DbContextOptions<BotContext> options) : base(options)
    {
        
    }
}