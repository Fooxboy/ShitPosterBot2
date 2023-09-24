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
        
        var files = Directory.GetFiles("DB");

        await botClient.SendTextMessageAsync(msg.From.Id, "Отправка базы данных");
            
        foreach (string file in files)
        {
            Thread.Sleep(5000);
            var a = file.Split('/');
            
            await using Stream stream = System.IO.File.OpenRead(file);
            await botClient.SendDocumentAsync(msg.From.Id, new InputOnlineFile(stream, a[^1]));
        }

        await botClient.SendTextMessageAsync(msg.From.Id, "Отправка базы даных успешно обработана");
    }
}