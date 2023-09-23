using ShitPosterBot2.Shared.Models;
using VkNet.Model;

namespace ShitPosterBot2.Collector.VKontakte.Hadlers;

public class GifHandler : IAttachmentHandler
{
    public Type HandlerType => typeof(Document);

    public async Task<PostAttachment?> Handle(MediaAttachment attachVk)
    {
        var gif = attachVk as Document;

        var attachment = new PostAttachment();

        attachment.AttachmentType = AttachmentType.Gif;
        attachment.Url = gif.Uri;

        return attachment;
    }
}