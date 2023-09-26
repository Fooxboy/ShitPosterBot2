using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.UserHandlers;

public interface IUserHandler
{
    
    public HandlerType HandlerType { get; }
    
    public Task Handle(Update tgUpdate, ITelegramBotClient botClient,
        TelegramMessageHandlerConfiguration configuration);
}