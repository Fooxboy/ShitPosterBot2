using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Shared;

public interface IPostRepository
{
    public Task<Post?> FindPostById(long id);

    public Task<Post?> AddPostAsync(Post post);

    public Task UpdateSentTime(Post dbPost, DateTime time);

    public Task<List<Post>> GetRandomPosts(int count);
}