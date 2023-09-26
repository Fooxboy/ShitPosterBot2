using Microsoft.Extensions.Logging;
using ShitPosterBot2.MessageHandler.UserHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler;

public class TelegramMessageHandler : IMessageHandler
{
    private TelegramBotClient _botClient;
    private TelegramMessageHandlerConfiguration _configuration;
    private readonly IEnumerable<IUserHandler> _userHandlers;

    private readonly ILogger<TelegramMessageHandler> _logger;

    public TelegramMessageHandler(ILogger<TelegramMessageHandler> logger, IEnumerable<IUserHandler> userHandlers)
    {
        _logger = logger;
        _userHandlers = userHandlers;
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
        if (update.Message is not null)
        {
            await InvokeHandler(update, HandlerType.Message);
        }

        if(update.Type is Telegram.Bot.Types.Enums.UpdateType.InlineQuery && update.InlineQuery != null)
        {
            await InvokeHandler(update, HandlerType.InlineQuery);
        }
    }

    private async Task InvokeHandler(Update update, HandlerType handlerType)
    {
        var handler = _userHandlers.FirstOrDefault(x => x.HandlerType == handlerType);

        if (handler is null)
        {
            _logger.LogError($"Не найден хендлер {handlerType}");

            return;
        }
            
        try
        {
            await handler.Handle(update, _botClient, _configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при выполнении хендлера {handlerType}: {ex.Message}");
            _logger.LogError(ex, ex.Message);
        }
    }
   
    private Task HandleErrorAsync(ITelegramBotClient arg1, Exception ex, CancellationToken arg3)
    {
        _logger.LogError(ex, ex.Message);

        return Task.CompletedTask;
    }
    
}