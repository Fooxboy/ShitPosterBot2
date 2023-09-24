using Microsoft.Extensions.DependencyInjection;
using ShitPosterBot2.Database;

namespace ShitPosterBot2.Repositories;

public class TopSecretsRepository
{
    private readonly BotContext _botContext;


    public TopSecretsRepository(BotContext botContext)
    {
        _botContext = botContext;
    }

    public string? GetSecret(string key)
    {
        var secretValue = _botContext.TopSecrets.FirstOrDefault(t => t.Key == key);

        return secretValue?.Value;
    }
}