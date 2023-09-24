using Microsoft.EntityFrameworkCore;
using ShitPosterBot2.Database;
using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.ExternValidators;

public class VkExternValidator : IExternPostValidator
{
    private readonly BotContext _botContext;

    public VkExternValidator(BotContext botContext)
    {
        _botContext = botContext;
    }

    public async Task<bool> IsValid(Post post)
    {
        return !await _botContext.Posts.AnyAsync(x => x.PlatformOwner == post.PlatformOwner && x.PlatformId == post.PlatformId);
    }
}