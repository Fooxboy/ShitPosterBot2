using ShitPosterBot2.Backupper;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace ShitPosterBot2.MessageHandler.Commands;

public class SenddbCommand : IUserCommand
{
    public string Name => "senddb";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        if (495716470 != msg.From.Id)
        {
            await botClient.SendTextMessageAsync(msg.From.Id, "Не не не, тебе к этому нельзя иметь доступ"); 
            return;
        }

        var sender = new DatabaseSender();

        var dbDirectory = Path.Combine(Environment.CurrentDirectory, "backups");
        await sender.SendDatabase(dbDirectory, botClient, msg.Chat.Id.ToString());
    }
}