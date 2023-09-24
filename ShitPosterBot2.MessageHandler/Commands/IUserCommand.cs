using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public interface IUserCommand
{
    public string Name { get; }

    public Task Invoke(Message msg, ITelegramBotClient botClient);
}