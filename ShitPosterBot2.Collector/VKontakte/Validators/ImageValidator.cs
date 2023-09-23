using System.Diagnostics;
using Microsoft.Extensions.Logging;
using VkNet.Model;

namespace ShitPosterBot2.Collector.VKontakte.Validators;

public class ImageValidator : IAttachmentValidator
{
    public Type ValidationType => typeof(Photo);

    public async Task<ValidationResult> ValidateAsync(MediaAttachment attachVk)
    {
        var photo = attachVk as Photo;

        var maxphoto = photo.Sizes.MaxBy(s => s.Height);

        if (maxphoto is null)
        {
            return new ValidationResult() { IsSuccess = false, Message = "MaxSize был  null" };
        }

        if (maxphoto.Height <= 150 || maxphoto.Width <= 150)
        {
            return new ValidationResult() { IsSuccess = false, Message = "Размер фотографии меньше чем 150х150" };
        }
        
        if (CheckSizeImage(maxphoto?.Url.ToString()))
        {
            return new ValidationResult() { IsSuccess = false, Message = "Размер фотографии слишком маленький" };

        }

        var url = maxphoto?.Url;

        if (url == null)
        {
            return new ValidationResult() { IsSuccess = false, Message = "Нет ссылки на фотографию" };
        }

        return new ValidationResult() {IsSuccess = true, Message = "Все отлично :)"};
    }

    private bool CheckSizeImage(string link)
    {
        var particles = link.Split('&');

        var a = particles[particles.Length - 1];

        var r = a.Contains("size");

        return a.Contains("size");
    }
}