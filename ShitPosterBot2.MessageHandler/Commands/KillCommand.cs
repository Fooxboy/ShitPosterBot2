using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public class KillCommand : IUserCommand
{
    public string Name => "kill";
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        
        if (495716470 != msg.From.Id)
        {
            await botClient.SendTextMessageAsync(msg.From.Id, "Ненене тебе сюда ннельзя кыш");

            return;
        }

        await botClient.SendTextMessageAsync(msg.From.Id, "Убийство текущего процесса");
        
        Process processkill = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "kill",
            Arguments = $"-9 {Process.GetCurrentProcess().Id}"
        };
        processkill.StartInfo = startInfo;
        processkill.Start();
    }
}