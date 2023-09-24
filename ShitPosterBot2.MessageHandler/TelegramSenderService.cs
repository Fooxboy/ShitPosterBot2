using ShitPosterBot2.Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ShitPosterBot2.MessageHandler;

public class TelegramSenderService
{
    private readonly ITelegramBotClient _botClient;
    
    public TelegramSenderService(ITelegramBotClient client)
    {
        _botClient = client;
    }

    public async Task SendPostAsync(Post post, string text, string target)
    {
        if (post.Attachments.Count == 1)
        {
            await SendSinglePostAsync(post, text, target);
            return;
        }
        
        var gifAndImage = post.Attachments.Any(a => a.AttachmentType == AttachmentType.Gif) && post.Attachments.Any(a => a.AttachmentType == AttachmentType.Image);
        var gifAndVideo = post.Attachments.Any(a => a.AttachmentType == AttachmentType.Gif) && post.Attachments.Any(a => a.AttachmentType == AttachmentType.Video);

        if (gifAndVideo || gifAndImage)
        {
            foreach (var attachment in post.Attachments)
            {
                await SendSinglePostAsync(post, text, target, attachment);
            }

            return;
        }

        await SendGroupedPostAsync(post, text, target);
    }

    public async Task SendGroupedPostAsync(Post post, string text, string target)
    {
        var files = new List<IAlbumInputMedia>();

        var attachments = post.Attachments;
        
        foreach (var attachment in attachments)
        {
            IAlbumInputMedia content = null;

            var isFirst = attachments.IndexOf(attachment) == 0;

            switch (attachment.AttachmentType)
            {
                case AttachmentType.Image:
                    
                    content = isFirst ? new InputMediaPhoto(attachment.Url) { Caption = text, ParseMode = ParseMode.Markdown } 
                        : new InputMediaPhoto(attachment.Url) { };

                    break;
                case AttachmentType.Gif:

                    content = isFirst ? new InputMediaDocument(attachment.Url) { Caption = text, ParseMode = ParseMode.Markdown }
                        : new InputMediaDocument(attachment.Url);

                    break;
                case AttachmentType.Video:

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(attachment.Url);
                        var fileStream = await response.Content.ReadAsStreamAsync();
                        
                        content = isFirst ? new InputMediaVideo(new InputMedia(fileStream, Guid.NewGuid().ToString())) { Caption = text, ParseMode = ParseMode.Markdown } 
                            : new InputMediaVideo(new InputMedia(fileStream, Guid.NewGuid().ToString()));
                    }
                
                    break;
            }

            if (content is null)
            {
                throw new Exception("Ошибка при создании сообщения с группировкой - content is null, lol");
            }
            
            files.Add(content);
        }
        
        await _botClient.SendMediaGroupAsync(target, files);
    }

    public async Task SendSinglePostAsync(Post post,  string text, string target, PostAttachment? attachmentToSend = null)
    {
        var attachment = attachmentToSend ?? post.Attachments.FirstOrDefault();
        
        switch(attachment.AttachmentType)
        {
            case AttachmentType.Image:
                await _botClient.SendPhotoAsync(target, attachment.Url, caption: text, parseMode: ParseMode.Markdown);
                break;
            case AttachmentType.Gif:
                await _botClient.SendAnimationAsync(target, attachment.Url, caption: text, parseMode: ParseMode.Markdown);
                break;
            case AttachmentType.Video:
                using (var httpClient = new HttpClient())
                {
                    var fileStream = await httpClient.GetStreamAsync(attachment.Url);
                    await _botClient.SendVideoAsync(target, new InputMedia(fileStream, Guid.NewGuid().ToString()), caption: text, parseMode: ParseMode.Markdown);
                }
              
                break;
            case AttachmentType.Unknown:
                throw new Exception("Неизвестное вложение лол");
        }
    }
}