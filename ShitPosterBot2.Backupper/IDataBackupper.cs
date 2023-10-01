namespace ShitPosterBot2.Backupper;

public interface IDataBackupper
{
    public Task Run(IDataBackupperConfiguration configuration);

    public Task Stop();
}