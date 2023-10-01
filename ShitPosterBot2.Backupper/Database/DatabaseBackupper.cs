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
            _logger.LogInformation("Отправка базы данных в телеграм...");

            try
            {
                var files = Directory.GetFiles(_configuration.DatabaseDirectory);

                if (!files.Any())
                {
                    throw new InvalidDataException("Не найдены файлы в папке где должно лежать бд");
                }
                
                var tgFiles = files.Select(localFile =>
                {
                    using var stream = System.IO.File.OpenRead(localFile);
                    return new InputMediaDocument(new InputMedia(stream, Guid.NewGuid().ToString()));
                });

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