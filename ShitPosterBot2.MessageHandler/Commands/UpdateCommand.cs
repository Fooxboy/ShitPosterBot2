using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public class UpdateCommand : IUserCommand
{
    public string Name => "update";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        if (495716470 != msg.From.Id)
        {
            await botClient.SendTextMessageAsync(msg.From.Id, "ты не можешь обновить бота");

            return;
        }

        await botClient.SendTextMessageAsync(msg.From.Id, "Выполняется запуск обновлениZ");
        
        var updater = @"/media/ShitFolder/LiisUpdateService/LiisUpdateService.dll";
        var linkcfg = "https://raw.githubusercontent.com/Liis17/links/main/Link/test/link.cfg";
        var currentversion = 0;
        var workfolder = @"/media/ShitFolder/Bot/";
        var password = "R1utherford#Curie#Maxwell42";
        var exfile = "ShitPosterBotConsole.dll";
        Process.Start(new ProcessStartInfo { FileName = "dotnet", Arguments = $"{updater} {linkcfg} {currentversion} {workfolder} {password} {exfile} {Process.GetCurrentProcess().Id}" });
    }
}