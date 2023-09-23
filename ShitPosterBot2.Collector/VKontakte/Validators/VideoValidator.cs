using VkNet.Model;

namespace ShitPosterBot2.Collector.VKontakte.Validators;

public class VideoValidator : IAttachmentValidator
{
    private readonly PostManager _vkPostManager;

    public VideoValidator(PostManager vkPostManager)
    {
        _vkPostManager = vkPostManager;
    }
    
    public Type ValidationType => typeof(Video);

    public async Task<ValidationResult> ValidateAsync(MediaAttachment attachVk)
    {
        Thread.Sleep(2000);
        try
        {
            if (attachVk is Video vkVideo)
            {
                Video vid;
                if (vkVideo.Files is null || vkVideo.Files.Mp4_480 is null || vkVideo.Files.Mp4_720 is null)
                {
                    vid = await _vkPostManager.GetVideo(attachVk as Video);
                }
                else
                {
                    vid = vkVideo;
                }

                var height = vid.Height;
                var videoUrl = string.Empty;

                if (vid.Files is null)
                {
                    return new ValidationResult()
                        { IsSuccess = false, Message = "Массив со списком файлов видеозаписи = null" };
                }

                switch (height)
                {
                    case 360:
                        videoUrl = vid.Files.Mp4_360.ToString();
                        break;
                    case 480:
                        videoUrl = vid.Files.Mp4_480.ToString();
                        break;
                    case 720:
                        videoUrl = vid.Files.Mp4_720.ToString();
                        break;
                    case 1080:
                        videoUrl = vid.Files.Mp4_1080.ToString();
                        break;
                }

                if (string.IsNullOrEmpty(videoUrl))
                    return new ValidationResult() { IsSuccess = false, Message = "Ссылка на видео = null" };
            }

        }
        catch (Exception ex)
        {
            return new ValidationResult() { IsSuccess = false, Message = $"Что то с видосом случилось: {ex}" };

        }

        return new ValidationResult() { IsSuccess = true, Message = "Все заебись" };
    }

}