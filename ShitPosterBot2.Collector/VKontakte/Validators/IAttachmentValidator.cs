using VkNet.Model;

namespace ShitPosterBot2.Collector.VKontakte.Validators;

public interface IAttachmentValidator
{
    public Type ValidationType { get; }

    public Task<ValidationResult> ValidateAsync(MediaAttachment attachVk);
}