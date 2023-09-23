using Microsoft.Extensions.Logging;
using ShitPosterBot2.Shared.Models;
using VkNet.Model;

namespace ShitPosterBot2.Collector.VKontakte.Hadlers;

public class VideoHandler : IAttachmentHandler
{
     public Type HandlerType => typeof(Video);

     private readonly PostManager _vkPostManager;
     
     public VideoHandler(PostManager vkPostManager)
     {
         _vkPostManager = vkPostManager;
     }

    public async Task<PostAttachment?> Handle(MediaAttachment attachVk)
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
            var attachment = new PostAttachment();
            attachment.AttachmentType = AttachmentType.Video;
    
            try
            {
                switch (height)
                {
                    case 360:
                        try
                        {
                            videoUrl = vid.Files.Mp4_360.ToString();
                        }
                        catch (Exception ex)
                        {
                            
                        }
                        break;
                    case 480:
                        try
                        {
                            videoUrl = vid.Files.Mp4_480.ToString();
                        }
                        catch (Exception ex)
                        {
                            
                        }
    
                        break;
                    case 720:
                        try
                        {
                            videoUrl = vid.Files.Mp4_480.ToString();
                        }
                        catch (Exception ex)
                        {
                            
                        }
    
                        break;
                    case 1080:
                        videoUrl = vid.Files.Mp4_480.ToString();
                        break;
                }
            }
            catch
            {
                try
                {
                    videoUrl = vid.Files.Mp4_360.ToString();
                }
                catch
                {
                    videoUrl = null;
                }
            }
            
    
            attachment.Url = videoUrl;
    
            return attachment;
        }


        return null;
    }
}