using FolderSync.Utils;

namespace FolderSync.Validators;

public class FileValidator
{
    public bool Validate(string sourcePath, string replicaPath)
    {
        if (!File.Exists(sourcePath) || !File.Exists(replicaPath))
            return false;
        
        var srcInfo = new FileInfo(sourcePath);
        var repInfo = new FileInfo(replicaPath);
        
        if (srcInfo.Length != repInfo.Length)
            return false;
            
        var srcHash = HashUtils.ComputeSHA256(sourcePath);
        var repHash = HashUtils.ComputeSHA256(replicaPath);

        return srcHash == repHash;
    }
}