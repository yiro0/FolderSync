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

    public void Synchronize()
    {
        _logger.Log("Starting synchronization...");

        var fileChanges = _fileTracker.CompareFiles(_sourcePath, _replicaPath);
        var dirChanges = _fileTracker.CompareDirectories(_sourcePath, _replicaPath);

        foreach (var file in fileChanges.NewFiles)
        {
            CopyFile(file);
        }

        foreach (var file in fileChanges.ModifiedFiles)
        {
            if (_fileValidator.Validate(file.SourcePath, file.ReplicaPath))
                CopyFile(file);
            else
                _logger.Log($"Validation failed for: {file.SourcePath}");
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

        _lastSyncTime = DateTime.Now;
        _logger.Log("Synchronization completed.");
    }

    private void CopyFile(FileChange file)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file.ReplicaPath));
            File.Copy(file.SourcePath, file.ReplicaPath, true);
            _logger.Log($"Copied file: {file.SourcePath} to {file.ReplicaPath}");
            
            if (!_fileValidator.Validate(file.SourcePath, file.ReplicaPath))
            {
                _logger.Log($"Validation failed after copy for: {file.SourcePath}");
            }
        }
        catch (Exception ex)
        {
            _logger.Log($"Error copying file {file.SourcePath}: {ex.Message}");
        }
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