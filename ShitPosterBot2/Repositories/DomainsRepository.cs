using ShitPosterBot2.Database;

namespace ShitPosterBot2.Repositories;

public class DomainsRepository
{
    private readonly BotContext _botContext;
    
    public DomainsRepository(BotContext botContext)
    {
        _botContext = botContext;
    }

    public List<string> GetAllDomains()
    {
        return _botContext.Domains.Select(d => d.Name).ToList();
    }
}