using FolderSync.Models;

namespace FolderSync.Trackers;

public class DirectoryChanges
{
    public List<DirectoryChange> NewDirectories { get; set; } = new();
    public List<DirectoryChange> DeletedDirectories { get; set; } = new();
}