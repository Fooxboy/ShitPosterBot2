using Microsoft.Extensions.Logging;
using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.UserHandlers;

public class UserMessageHandler : IUserHandler
{
    private readonly CommandsManager _commandsManager;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<UserMessageHandler> _logger;
    private TelegramMessageHandlerConfiguration _configuration;
    private ITelegramBotClient _botClient;

    public UserMessageHandler(CommandsManager commandsManager, IPostRepository postRepository, ILogger<UserMessageHandler> logger)
    {
        _commandsManager = commandsManager;
        _postRepository = postRepository;
        _logger = logger;
    }

    public HandlerType HandlerType => HandlerType.Message;

    public async Task Handle(Update tgUpdate, ITelegramBotClient client, 
        TelegramMessageHandlerConfiguration configuration)
    {
        _configuration = configuration;
        _botClient = client;

        if (tgUpdate.Message is not Message msg)
        {
            return;
        }

        var text = msg.Text;
        
        _logger.LogInformation($"Обработка сообщения с текстом {text}");

        if (text is null) return;

        if (text.StartsWith("/"))
        {
            text = text.Replace("/", string.Empty);
            
            await _commandsManager.ProccessCommand(text, msg, client);

            return;
        }
        
        if (long.TryParse(text, out var postId))
        {
            await HandleGetPostRequest(postId, msg);
        }
    }

    private async Task HandleGetPostRequest(long postId, Message msg)
    {
        var post = await _postRepository.FindPostById(postId);

        if (post is null)
        {
            await _botClient.SendTextMessageAsync(msg.Chat.Id, $"Пост с ID = {postId} не найден");

            return;
        }

        var senderService = new TelegramSenderService(_botClient);

        _logger.LogInformation($"Отправка поста {post.Id} пользователю");
        Thread.Sleep(_configuration.SendMessageUserTimeout);
        
        await senderService.SendPostAsync(post, $"Вот твой пост с ID {post.Id}", msg.Chat.Id.ToString());

        _logger.LogInformation($"Отправка документов с поста {post.Id} пользователю");
        
        foreach (var postAttachment in post.Attachments.Where(postAttachment => postAttachment.AttachmentType != AttachmentType.Video 
                     && postAttachment.Url is not null))
        {
            
            Thread.Sleep(_configuration.SendMessageUserTimeout);
            await _botClient.SendDocumentAsync(msg.Chat.Id, postAttachment?.Url!);
        }
    }
}