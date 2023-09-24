using Microsoft.EntityFrameworkCore;
using ShitPosterBot2.Database;
using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Repositories;

public class PostsRepository
{
    private readonly BotContext _botContext;

    public PostsRepository(BotContext botContext)
    {
        _botContext = botContext;
    }

    public async Task<Post?> AddPostAsync(Post post)
    {
        var dbPostExist = await _botContext.Posts.FirstOrDefaultAsync(x => x.PlatformId == post.PlatformId);

        if (dbPostExist != null)
        {
            return dbPostExist;
        }
        
        var domain = _botContext.Domains.FirstOrDefault(x => x.Name == post.PlatformOwner);

        if (domain is null)
        {
            throw new Exception($"Не найден домен {post.PlatformOwner} для связи лол");
        }

        post.DomainInfo = domain;
        _botContext.Posts.Add(post);

        await _botContext.SaveChangesAsync();

        return _botContext.Posts.Where(x => x.PlatformId == post.PlatformId)
            .Include(p=> p.DomainInfo)
            .FirstOrDefault();
    }

    public async Task UpdateSentTime(Post dbPost, DateTime time)
    {
        var post = _botContext.Posts.FirstOrDefault(x => x.Id == dbPost.Id);

        if (post is null)
        {
            throw new Exception($"Не найден пост с ID {dbPost.Id}");
        }
        
        post.PublishAt = time;

        await _botContext.SaveChangesAsync();
    }
}