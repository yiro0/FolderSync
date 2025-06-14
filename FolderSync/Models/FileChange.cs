namespace FolderSync.Models;

public class FileChange
{
    public string? SourcePath { get; set; }
    public string ReplicaPath { get; set; }
}