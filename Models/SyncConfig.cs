namespace FolderSync.Models
{
    public record SyncConfig(string SourcePath, string ReplicaPath, int IntervalSeconds, string LogFilePath);
}
