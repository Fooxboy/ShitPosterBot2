using Microsoft.EntityFrameworkCore;
using ShitPosterBot2.Database;
using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Repositories;

public class PostsRepository : IPostRepository
{
    private readonly BotContext _botContext;

    public PostsRepository(BotContext botContext)
    {
        _botContext = botContext;
    }

    public async Task<Post?> FindPostById(long id)
    {
        var post = await _botContext.Posts.Where(post => post.Id == id).Include(post => post.Attachments).FirstOrDefaultAsync();

        return post;
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

    public async Task<List<Post>> GetRandomPosts(int count)
    {
        var countPosts = await _botContext.Posts.CountAsync();

        var randomIds = new List<long>();

        for (int i = 0; i < count; i++)
        {
            randomIds.Add(Random.Shared.Next(0, countPosts));
        }

        var posts = await _botContext.Posts.Where(post => randomIds.Contains(post.Id)).Include(post=> post.Attachments).ToListAsync();

        return posts;
    }
}