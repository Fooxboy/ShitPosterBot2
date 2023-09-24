using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public class ChannelCommand : IUserCommand
{
    public string Name => "channel";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(msg.Chat.Id,
            $"Вот основной канал \n@fuckedupmemeschannel\nhttps://t.me/fuckedupmemeschannel");
    }
}