using ShitPosterBot2.Shared.Models;
using VkNet.Model;

namespace ShitPosterBot2.Collector.VKontakte.Hadlers;

public interface IAttachmentHandler
{
    public Type HandlerType { get; }

    public Task<PostAttachment?> Handle(MediaAttachment attachVk);
}