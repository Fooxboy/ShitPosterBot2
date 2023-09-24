using Microsoft.Extensions.Logging;
using ShitPosterBot2.Shared;
using ShitPosterBot2.Shared.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler;

public class TelegramMessageHandler : IMessageHandler
{
    private TelegramBotClient _botClient;
    private TelegramMessageHandlerConfiguration _configuration;
    private readonly CommandsManager _commandsManager;
    private readonly IPostRepository _postRepository;

    private readonly ILogger<TelegramMessageHandler> _logger;

    public TelegramMessageHandler(ILogger<TelegramMessageHandler> logger, CommandsManager commandsManager,
        IPostRepository postRepository)
    {
        _logger = logger;
        _commandsManager = commandsManager;
        _postRepository = postRepository;
    }
    
    
    public async Task RunMesageHandler(IMessageHandlerConfiguration configuration)
    {

        if (configuration is not TelegramMessageHandlerConfiguration tgConfig)
        {
            _logger.LogError("Неверные параметры переданы были. Остановка...");

            return;
        }

        _configuration = tgConfig;
        
        _botClient = new TelegramBotClient(_configuration.TelegramToken);
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);

    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Message is Message message)
        {
            _logger.LogInformation("Пришло новое сообщение");

            await ProcessNewMessage(client, message);
        }

        if(update.Type is Telegram.Bot.Types.Enums.UpdateType.InlineQuery)
        {
            _logger.LogInformation("Пришёл инлайн запрос");
        }
    }

    private async Task ProcessNewMessage(ITelegramBotClient client, Message msg)
    {
        var text = msg.Text;

        if (text.StartsWith("/"))
        {
            text = text.Replace("/", string.Empty);
            
            await _commandsManager.ProccessCommand(text, msg, _botClient);

            return;
        }
        
        _logger.LogInformation($"Обработка сообщения с текстом {text}");

        if (long.TryParse(text, out var postId))
        {
            var post = await _postRepository.FindPostById(postId);

            if (post is null)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, $"Пост с ID = {postId} не найден");

                return;
            }

            var senderService = new TelegramSenderService(client);

            
            _logger.LogInformation($"Отправка поста {post.Id} пользователю");
            Thread.Sleep(_configuration.SendMessageUserTimeout);
            await senderService.SendPostAsync(post, $"Вот твой пост с ID {post.Id}", msg.Chat.Id.ToString());

            _logger.LogInformation($"Отправка документов с поста {post.Id} пользователю");
            foreach (var postAttachment in post.Attachments)
            {
                if (postAttachment.AttachmentType == AttachmentType.Video) continue;

                Thread.Sleep(_configuration.SendMessageUserTimeout);
                await client.SendDocumentAsync(msg.Chat.Id, postAttachment.Url);
            }

            return;

        }
        
    }
    
    private Task HandleErrorAsync(ITelegramBotClient arg1, Exception ex, CancellationToken arg3)
    {
        _logger.LogError(ex, ex.Message);

        return Task.CompletedTask;
    }
    
}