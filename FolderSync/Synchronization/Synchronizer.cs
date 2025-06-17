using FolderSync.Logging;
using FolderSync.Models;
using FolderSync.Trackers;
using FolderSync.Validators;

namespace FolderSync.Synchronization;

public class Synchronizer
{
    private readonly FileTracker _fileTracker;
    private readonly FileValidator _fileValidator;
    private readonly string _sourcePath;
    private readonly string _replicaPath;
    private readonly Logger _logger;
    private DateTime _lastSyncTime;

    public Synchronizer(
        string sourcePath,
        string replicaPath,
        FileTracker fileTracker,
        FileValidator fileValidator,
        Logger logger)
    {
        _sourcePath = sourcePath;
        _replicaPath = replicaPath;
        _fileTracker = fileTracker;
        _fileValidator = fileValidator;
        _logger = logger;
        _lastSyncTime = DateTime.MinValue;
    }

    private readonly HashSet<string> _failedValidations = new();

public void Synchronize()
{
    _logger.Log("Starting synchronization...");

    var fileChanges = _fileTracker.CompareFiles(_sourcePath, _replicaPath);
    var dirChanges = _fileTracker.CompareDirectories(_sourcePath, _replicaPath);

    foreach (var file in fileChanges.NewFiles)
    {
        CopyFile(file, "Copied file");
    }

    foreach (var file in fileChanges.ModifiedFiles)
    {
        CopyFile(file, "Modified file");
    }

    foreach (var file in fileChanges.DeletedFiles)
    {
        DeleteFile(file);
    }

    foreach (var dir in dirChanges.NewDirectories)
    {
        CreateDirectory(dir);
    }

    foreach (var dir in dirChanges.DeletedDirectories)
    {
        DeleteDirectory(dir);
    }
    
    foreach (var path in _failedValidations.ToList())
    {
        if (_fileValidator.Validate(path, GetReplicaPath(path)))
        {
            _logger.Log($"Re-validation succeeded for: {path}");
            _failedValidations.Remove(path);
        }
        else
        {
            _logger.Log($"Re-validation failed for: {path}");
        }
    }

    _lastSyncTime = DateTime.Now;
    _logger.Log("Synchronization completed.");
}

private void CopyFile(FileChange file, string action)
{
    try
    {
        Directory.CreateDirectory(Path.GetDirectoryName(file.ReplicaPath));
        File.Copy(file.SourcePath, file.ReplicaPath, true);
        long bytes = new FileInfo(file.SourcePath).Length;
        _logger.Log($"{action}: {file.SourcePath} to {file.ReplicaPath} ({bytes} bytes)");

        if (!_fileValidator.Validate(file.SourcePath, file.ReplicaPath))
        {
            _logger.Log($"Validation failed after copy for: {file.SourcePath}");
            _failedValidations.Add(file.SourcePath);
        }
        else
        {
            _failedValidations.Remove(file.SourcePath);
        }
    }
    catch (Exception ex)
    {
        _logger.Log($"Error copying file {file.SourcePath}: {ex.Message}");
    }
}

private string GetReplicaPath(string sourcePath)
{
    return sourcePath.Replace(_sourcePath, _replicaPath);
}

    private void DeleteFile(FileChange file)
    {
        try
        {
            if (File.Exists(file.ReplicaPath))
            {
                File.Delete(file.ReplicaPath);
                _logger.Log($"Deleted file: {file.ReplicaPath}");
            }
        }
        catch (Exception ex)
        {
            _logger.Log($"Error deleting file {file.ReplicaPath}: {ex.Message}");
        }
    }

    private void CreateDirectory(DirectoryChange dir)
    {
        try
        {
            Directory.CreateDirectory(dir.ReplicaPath);
            _logger.Log($"Created directory: {dir.ReplicaPath}");
        }
        catch (Exception ex)
        {
            _logger.Log($"Error creating directory {dir.ReplicaPath}: {ex.Message}");
        }
    }

    private void DeleteDirectory(DirectoryChange dir)
    {
        try
        {
            if (Directory.Exists(dir.ReplicaPath))
            {
                Directory.Delete(dir.ReplicaPath, true);
                _logger.Log($"Deleted directory: {dir.ReplicaPath}");
            }
        }
        catch (Exception ex)
        {
            _logger.Log($"Error deleting derectory {dir.ReplicaPath}: {ex.Message}");
        }
    }
}