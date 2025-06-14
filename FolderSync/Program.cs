using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FolderSync.Logging;

class Program
{
    static void Main(string[] args)
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