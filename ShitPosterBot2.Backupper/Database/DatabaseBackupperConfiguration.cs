namespace ShitPosterBot2.Backupper.Database;

public class DatabaseBackupperConfiguration : IDataBackupperConfiguration
{
    public string DatabaseDirectory { get; set; }
    
    public int Timeout { get; set; }
    
    public string TelegramBotToken { get; set; }
    
    public string TargetId { get; set; }
}