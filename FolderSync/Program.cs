using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FolderSync.Logging;
using FolderSync.Synchronization;
using FolderSync.Trackers;
using FolderSync.Validators;

class Program
{
    static async Task Main(string[] args)
    {
        var argDict = ParseArgs(args);

        if (!argDict.ContainsKey("source") ||
            !argDict.ContainsKey("replica") ||
            !argDict.ContainsKey("interval") ||
            !argDict.ContainsKey("log"))
        {
            Console.WriteLine(
                "Usage: FolderSync --source <source> --replica <replica> --interval <intervalSeconds> --log <logFilePath>");
            return;
        }

        string sourcePath = argDict["source"];
        string replicaPath = argDict["replica"];

        if (!int.TryParse(argDict["interval"], out int intervalSeconds))
        {
            Console.WriteLine("Invalid interval value.");
            return;
        }

        if (!Directory.Exists(sourcePath))
        {
            Console.WriteLine("Source directory does not exist.");
            return;
        }

        Directory.CreateDirectory(replicaPath);
        string logFilePath = argDict["log"];
        Logger logger = new Logger(logFilePath);

        var fileTracker = new FileTracker();
        var fileValidator = new FileValidator();

        var synchronizer = new Synchronizer(sourcePath, replicaPath, fileTracker, fileValidator, logger);
        Console.WriteLine("Press Ctrl+C to stop the program.");
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            Console.WriteLine("Stopping...");
        };

        while (!cts.Token.IsCancellationRequested)
        {
            synchronizer.Synchronize();
            try
            {
                await Task.Delay(intervalSeconds * 1000, cts.Token);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    static Dictionary<string, string> ParseArgs(string[] args)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].StartsWith("--"))
            {
                string key = args[i].Substring(2);
                string value = args[i + 1];
                dict[key] = value;
                i++;
            }
        }
        return dict;
    }
}