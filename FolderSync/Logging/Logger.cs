namespace FolderSync.Logging;

public class Logger : ILogger
{
    private readonly string _logFilePath;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public void Log(string message)
    {
        string logMessage = $"{DateTime.Now}: {message}";
        Console.WriteLine(logMessage);
        File.AppendAllText(_logFilePath,logMessage + Environment.NewLine);
    }
}