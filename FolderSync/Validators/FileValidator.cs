namespace FolderSync.Validators;

public class FileValidator
{
    public bool Validate(string sourcePath, string replicaPath)
    {
        if (!File.Exists(sourcePath) || !File.Exists(replicaPath))
            return false;
        var srcInfo = new FileInfo(sourcePath);
        var repInfo = new FileInfo(replicaPath);


        return srcInfo.Length == repInfo.Length &&
               File.GetLastWriteTimeUtc(sourcePath) == File.GetLastWriteTimeUtc(replicaPath);
    }
}