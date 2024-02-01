using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.Backupper;

public class DatabaseSender
{
    
    public async Task<string> SendDatabase(string dbDirectory, ITelegramBotClient telegramClient, string targetId)
    {
        var zipper = new Zipper();

        var pathToArchives = await zipper.ZipFilesInDirectory(dbDirectory);
                
        var backupFiles = Directory.GetFiles(Path.Combine(dbDirectory, "backups"));

        var tgFiles = backupFiles.Select(backup =>
        {
            using var stream = System.IO.File.OpenRead(backup);
            return new InputMediaDocument(new InputMedia(stream, Guid.NewGuid().ToString()));
        });
                
        await telegramClient.SendMediaGroupAsync(targetId, tgFiles);

        return pathToArchives;
    }
}