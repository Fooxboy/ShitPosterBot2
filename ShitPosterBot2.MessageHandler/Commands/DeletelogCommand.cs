using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public class DeletelogCommand : IUserCommand
{
    public string Name => "deletelog";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        if (495716470 != msg.From.Id)
        {
            await botClient.SendTextMessageAsync(msg.From.Id, "Не не не, тебе к этому нельзя иметь доступ");
            return;
        }

        await botClient.SendTextMessageAsync(msg.From.Id, "Удаление логов");
        
        var files = Directory.GetFiles("logs");
        
        foreach (string file in files)
        {
            try
            {
                System.IO.File.Delete(file);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(msg.From.Id, "Упало удаление логов лол");
            }
        }


        await botClient.SendTextMessageAsync(msg.From.Id, "Логи удалены");

    }
}