namespace FolderSync.Models;

public class ChangeSet
{
    public List<FileChange> NewFiles { get; set; } = new();
    public List<FileChange> ModifiedFiles { get; set; } = new();
    public List<FileChange> DeletedFiles { get; set; } = new();
    public List<DirectoryChange> NewDirectories { get; set; } = new();
    public List<DirectoryChange> DeletedDirectories { get; set; } = new();
    
    public ChangeSet Compare(string sourcePath, string replicaPath)
    {
        return new ChangeSet();
    }
}