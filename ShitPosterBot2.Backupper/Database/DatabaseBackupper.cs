using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShitPosterBot2.Backupper.Database;

public class DatabaseBackupper : IDataBackupper
{
    private readonly ILogger<DatabaseBackupper> _logger;
    private DatabaseBackupperConfiguration _configuration;
    private TelegramBotClient _telegramClient;
    private bool _isRun;

    public DatabaseBackupper(ILogger<DatabaseBackupper> logger)
    {
        _logger = logger;
    }

    public async Task Run(IDataBackupperConfiguration configuration)
    {
        _logger.LogInformation("Запуск сервиса отправки базы данных...");

        if (configuration is not DatabaseBackupperConfiguration dbConfig)
        {
            _logger.LogError("Были переданы неверные настройки для DatabaseBackupper");

            throw new ArgumentException("Были переданы неверные настройки для DatabaseBackupper ");
        }

        _configuration = dbConfig;

        _telegramClient = new TelegramBotClient(_configuration.TelegramBotToken);

        _isRun = true;

        while (_isRun)
        {

            try
            {
                _logger.LogInformation("Начало работы бд бэкаппера");
                
                var zipper = new Zipper();

                _logger.LogInformation("запаковка файлов");

                var pathToArchives = await zipper.ZipFilesInDirectory(_configuration.DatabaseDirectory);
                
                _logger.LogInformation("запаковка завершена");

                var backupFiles = Directory.GetFiles(Path.Combine(_configuration.DatabaseDirectory, "backups"));

                var tgFiles = backupFiles.Select(backup =>
                {
                    using var stream = System.IO.File.OpenRead(backup);
                    return new InputMediaDocument(new InputMedia(stream, Guid.NewGuid().ToString()));
                });
                
                _logger.LogInformation("Отправка базы данных в телеграм...");

                await _telegramClient.SendMediaGroupAsync(_configuration.TargetId, tgFiles);

                _logger.LogInformation("База данных успешно отправлена!");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                Thread.Sleep(_configuration.Timeout);
            }
        }
        
    }

    public Task Stop()
    {
        _isRun = false;

        return Task.CompletedTask;
    }
}