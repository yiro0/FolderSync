using FolderSync.Models;

namespace FolderSync.Trackers;

public class FileChanges
{
    public List<FileChange> NewFiles { get; set; } = new();
    public List<FileChange> ModifiedFiles { get; set; } = new();
    public List<FileChange> DeletedFiles { get; set; } = new();   
}