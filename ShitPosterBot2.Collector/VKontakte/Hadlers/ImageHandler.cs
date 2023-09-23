using ShitPosterBot2.Shared.Models;
using VkNet.Model;

namespace ShitPosterBot2.Collector.VKontakte.Hadlers;

public class ImageHandler : IAttachmentHandler
{
    public Type HandlerType => typeof(Photo);

    public async Task<PostAttachment?> Handle(MediaAttachment attachVk)
    {
        var photo = attachVk as Photo;

        var maxphoto = photo.Sizes.MaxBy(s => s.Height);

        var attachment = new PostAttachment();
        attachment.AttachmentType = AttachmentType.Image;
        attachment.Url = maxphoto.Url.ToString();

        return attachment;
    }
}