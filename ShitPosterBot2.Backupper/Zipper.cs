

using Ionic.Zip;

public class Zipper
{
    public async Task<string> ZipFilesInDirectory(string pathToDirectory, string saveDirectory)
    {
        var zipArchiveName = $"backup-{DateTime.UtcNow:yy-MM-dd}-{Random.Shared.Next(1, 20)}.zip";
        
        using (ZipFile zip = new ZipFile())
        {
            zip.AddDirectory(pathToDirectory);
            zip.MaxOutputSegmentSize = 40 * 1024 * 1024; // 100k segments
            zip.Save(Path.Combine(saveDirectory, zipArchiveName));
        }

        return zipArchiveName;
    }
}