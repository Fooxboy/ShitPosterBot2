using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Sender;

public interface IPostSender
{
    public int CountPostsInQueue { get; }

    public string Name { get; }

    public event Action<Exception, IPostSender> SenderCrashed;

    public event Func<Post, Task> PostSended;

    public Task AddToQueue(Post post);

    public Task RunSender(ISenderConfiguration configuration);

    public Task StopSender();

}