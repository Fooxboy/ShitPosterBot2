using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace ShitPosterBot2.MessageHandler.UserHandlers;

public class UserInlineQueryHandler : IUserHandler
{
    private readonly IPostRepository _postRepository;
    private ITelegramBotClient _botClient;

    public UserInlineQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public HandlerType HandlerType => HandlerType.InlineQuery;

    public async Task Handle(Update tgUpdate, ITelegramBotClient client, TelegramMessageHandlerConfiguration configuration)
    {
        _botClient = client;

        var inlineQuery = tgUpdate.InlineQuery;

        if (inlineQuery is null) return;

        if (string.IsNullOrEmpty(inlineQuery.Query))
        {
            await SendRandomPosts(inlineQuery.Id);

            return;
        }

        long postId = 0;
        if (long.TryParse(inlineQuery.Query, out postId))
        {
            await SendPostAttachments(inlineQuery.Id, postId);

            return;
        }

        await SendBadRequest(inlineQuery.Id);
    }

    private async Task SendRandomPosts(string queryId)
    {
        var randomPosts = await _postRepository.GetRandomPosts(10);

        var photoAttachments = randomPosts.SelectMany(x =>
            x.Attachments.Where(a => a.AttachmentType == AttachmentType.Image)
                .Select(x => x.Url));

        var inlinePhotos = photoAttachments.Select(photoUrl =>
            new InlineQueryResultPhoto(Guid.NewGuid().ToString(), photoUrl, photoUrl));

        await _botClient.AnswerInlineQueryAsync(queryId, inlinePhotos);
    }

    private async Task SendPostAttachments(string queryId, long postId)
    {
        var post = await _postRepository.FindPostById(postId);

        if (post is null)
        {
            //если поста такого нет, выводим пидофила
            var queryImage = new InlineQueryResultPhoto(Guid.NewGuid().ToString(), "https://sun9-13.userapi.com/impg/S-Sp9A3PN1n0pHEpSc0uyg4ERyJEsKSRO14IyA/eKnSed9i67E.jpg?size=1024x1024&quality=95&sign=fc4d561929753bac5ea2b3582f067f48&type=album", "https://sun9-52.userapi.com/impg/-Mi6SiXlu5n4aI18zGRXkv8H7M3HXrzAzN6rCw/h_COjCQkQsA.jpg?size=1024x1024&quality=95&sign=b5ccc60daef68b88a1eb7e32236ad760&type=album");

            await _botClient.AnswerInlineQueryAsync(queryId, new[] { queryImage });

            return;
        }


        var inlineQueryPhotos = post.Attachments.Where(attachment => attachment.AttachmentType == AttachmentType.Image)
            .Select(attachment => attachment.Url).Select(photoUrl =>
                new InlineQueryResultPhoto(Guid.NewGuid().ToString(), photoUrl, photoUrl));

        if (inlineQueryPhotos.Any())
        {
            await _botClient.AnswerInlineQueryAsync(queryId, inlineQueryPhotos);
        }
    }

    private async Task SendBadRequest(string queryId)
    {
        //Если текст не является цифрвами выводим iq
        var queryImage = new InlineQueryResultPhoto(Guid.NewGuid().ToString(), "https://sun9-72.userapi.com/impg/WiCVN4uBUYHayZhwg4sh8AKejG4mKz_P6HPYdQ/81lvr08HHoY.jpg?size=1024x1024&quality=95&sign=f8ef1115aa7d4090cc6c1b6f9a619da5&type=album", "https://sun9-72.userapi.com/impg/E3DdB9KzAnQcw3GGaZcYEB5mjrMGrmx2EjxtoA/F7YPlSIszIw.jpg?size=1024x1024&quality=95&sign=fd1c39c8bc33910d4749e1c3ca9f2172&type=album");

        await _botClient.AnswerInlineQueryAsync(queryId, new[] { queryImage });
    }
}