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


        var databaseSender = new DatabaseSender();
        
        while (_isRun)
        {

            try
            {
                _logger.LogInformation("Начало работы бд бэкаппера");

                await databaseSender.SendDatabase(_configuration.DatabaseDirectory, _telegramClient, _configuration.TargetId);
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