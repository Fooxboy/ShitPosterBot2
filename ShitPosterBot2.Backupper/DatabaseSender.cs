using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace ShitPosterBot2.Backupper;

public class DatabaseSender
{
    
    public async Task<string> SendDatabase(string dbDirectory, ITelegramBotClient telegramClient, string targetId)
    {
        var pathToSave = Path.Combine(Environment.CurrentDirectory, "backups");

        Directory.CreateDirectory(pathToSave);

        var zipper = new Zipper();

        var pathToArchives = await zipper.ZipFilesInDirectory(dbDirectory, pathToSave);
                
        var backupFiles = Directory.GetFiles(pathToSave);

        foreach (var backupFile in backupFiles)
        {
            using (var streamFile = System.IO.File.OpenRead(backupFile))
            {
                var media = new InputMedia(streamFile, Path.GetFileName(backupFile));

                await telegramClient.SendDocumentAsync(targetId, media);
            }
            
            File.Delete(backupFile);
        }
                
        return pathToArchives;
    }
}