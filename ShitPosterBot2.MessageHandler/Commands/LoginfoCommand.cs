using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.MessageHandler.Commands;

public class LoginfoCommand : IUserCommand
{
    public string Name => "loginfo";
    
    public async Task Invoke(Message msg, ITelegramBotClient botClient)
    {
        
        if (495716470 != msg.From.Id)
        {
            await botClient.SendTextMessageAsync(msg.From.Id, "Не не не, тебе к этому нельзя иметь доступ");
            return;
        }
        
        long a = GetDirectorySize("logs");

        await botClient.SendTextMessageAsync(msg.From.Id, $"Размер папки в логами: {a / 1024 / 1024} мегабайт");
    }
    
    private long GetDirectorySize(string path)
    {
        // Если указанный путь не существует, вернуть 0
        if (!Directory.Exists(path))
            return 0;

        // Получить список файлов и папок в заданной директории
        var files = Directory.GetFiles(path);
        var dirs = Directory.GetDirectories(path);

        // Рассчитать размер всех файлов в папке
        long size = 0;
        foreach (string file in files)
        {
            var fileInfo = new FileInfo(file);
            size += fileInfo.Length;
        }

        // Рекурсивно рассчитать размер всех файлов в подпапках
        foreach (string dir in dirs)
        {
            size += GetDirectorySize(dir);
        }

        return size;
    }
}