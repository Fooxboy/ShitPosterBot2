using Microsoft.Extensions.Logging;

namespace ShitPosterBot2.Backupper.Logs;

public class LogsBackupper : IDataBackupper
{
    private readonly ILogger<LogsBackupper> _logger;
    
    private bool _isRun;

    public LogsBackupper(ILogger<LogsBackupper> logger)
    {
        _logger = logger;
    }

    public async Task Run(IDataBackupperConfiguration configuration)
    {
        _isRun = true;
    }

    public async Task Stop()
    {
        _isRun = false;
    }
}