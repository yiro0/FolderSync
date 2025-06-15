using FolderSync.Models;

namespace FolderSync.Trackers;

public class FileTracker
{
    public FileChanges CompareFiles(string sourcePath, string replicaPath)
    {
        var changes = new FileChanges();
        
        var sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
        var replicaFiles = Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories);
        
        var sourceSet = new HashSet<string>(sourceFiles.Select(f => f.Substring(sourcePath.Length).TrimStart('\\', '/')));
        var replicaSet = new HashSet<string>(replicaFiles.Select(f => f.Substring(replicaPath.Length).TrimStart('\\', '/')));

        //New and modified files
        foreach (var relpath in sourceSet)
        {
            var src = Path.Combine(replicaPath, relpath);
            var rep = Path.Combine(sourcePath, relpath);

            if (!replicaSet.Contains(relpath))
            {
                changes.NewFiles.Add(new FileChange {SourcePath = src, ReplicaPath = rep});
            }
            else if (File.GetLastWriteTimeUtc(src) != File.GetLastWriteTimeUtc(rep) ||
                     new FileInfo(src).Length != new FileInfo(rep).Length)
            {
                changes.ModifiedFiles.Add(new FileChange {SourcePath = src, ReplicaPath = rep});
            }
        }

        //Deleted files
        foreach (var relpath in replicaSet.Except(sourceSet))
        {
            var rep = Path.Combine(replicaPath, relpath);
            changes.DeletedFiles.Add(new FileChange { SourcePath = null, ReplicaPath = rep });
        }
        
        return changes;
    }

    public DirectoryChanges CompareDirectories(string sourcePath, string replicaPath)
    {
        var changes = new DirectoryChanges();
        //Directories
        var sourceDirs = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)
            .Select(d => d.Substring(sourcePath.Length).TrimStart('\\', '/')).ToHashSet();
        var replicaDirs = Directory.GetDirectories(replicaPath, "*", SearchOption.AllDirectories)
            .Select(d => d.Substring(replicaPath.Length).TrimStart('\\', '/')).ToHashSet();

        foreach (var relPath in sourceDirs.Except(replicaDirs))
        {
            changes.NewDirectories.Add(new DirectoryChange
            {
                SourcePath = Path.Combine(sourcePath, relPath),
                ReplicaPath = Path.Combine(replicaPath, relPath)
            });
        }

        foreach (var relPath in replicaDirs.Except(sourceDirs))
        {
            changes.DeletedDirectories.Add(new DirectoryChange
            {
                SourcePath = null,
                ReplicaPath = Path.Combine(replicaPath, relPath)
            });
        }

        return changes;
    }
}