using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace ShitPosterBot2.MessageHandler.Commands;

public class SendlogCommand : IUserCommand
{
    public string Name => "sendlog";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        if (495716470 != msg.From.Id)
        {
            await botClient.SendTextMessageAsync(msg.From.Id, "ненене тебе сюджа нельзя");
            
            return;
        }

        await botClient.SendTextMessageAsync(msg.From.Id, "отправка логов...");

        string[] files = Directory.GetFiles("logs");

        foreach (string file in files)
        {
            Thread.Sleep(5000);
            string[] a = file.Split('/');
            
            await using Stream stream = System.IO.File.OpenRead(file);

            await botClient.SendDocumentAsync(msg.From.Id, new InputOnlineFile(stream));
        }

        await botClient.SendTextMessageAsync(msg.From.Id, "логи отправлены!");
    }
}