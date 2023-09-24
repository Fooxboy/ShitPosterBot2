using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public class RestartCommand : IUserCommand
{
    public string Name => "restart";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        if (495716470 != msg.From.Id)
        {
            await botClient.SendTextMessageAsync(msg.From.Id, "Не не не, тебе к этому нельзя иметь доступ");
            return;
        }

        await botClient.SendTextMessageAsync(msg.From.Id, "Запуск ногово процесса");

        var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = $"{AppDomain.CurrentDomain.BaseDirectory}ShitPosterBotConsole.dll";
        process.Start();

        await botClient.SendTextMessageAsync(msg.From.Id, "Убийство текущего процесса");

        var processkill = new Process();
        var startInfo = new ProcessStartInfo()
        {
            FileName = "kill",
            Arguments = $"-9 {Process.GetCurrentProcess().Id}"
        };
        
        processkill.StartInfo = startInfo;
        processkill.Start();
    }
}