using VkNet.Model;

namespace ShitPosterBot2.Collector.VKontakte.Validators;

public class GifValidator : IAttachmentValidator
{
    public Type ValidationType => typeof(Document);

    public async Task<ValidationResult> ValidateAsync(MediaAttachment attachVk)
    {
        if (attachVk is Document gif)
        {
            if (gif.Uri == null)
            {
                return new ValidationResult() { IsSuccess = false, Message = "Нет ссылки на гифку" };
            }

            return new ValidationResult()
            {
                IsSuccess = gif is { Ext: "gif", Size: < 50000000 },
                Message = $"Расширение документа: {gif.Ext}, Размер: {gif.Size}"
            };
        }

        return new ValidationResult() { IsSuccess = false, Message = "Гифка не является документом (што)" };
    }
}