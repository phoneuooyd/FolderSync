using System;
using System.IO;
using FolderSync.Models;
using FolderSync.Exceptions;

namespace FolderSync.Utils
{
    public static class ArgumentParser
    {
        public static SyncConfig Parse(string[] args)
        {
            if (args.Length != 4)
                throw new InvalidArgumentsException("Expected arguments: <sourcePath> <replicaPath> <intervalSeconds> <logFilePath>");

            var source = args[0];
            var replica = args[1];
            if (!int.TryParse(args[2], out var interval) || interval <= 0)
                throw new InvalidArgumentsException("Interval must be a positive integer");
            var logFile = args[3];

            if (Directory.Exists(logFile))
                throw new InvalidArgumentsException($"Log file path points to a directory: {logFile}");

            var logDir = Path.GetDirectoryName(logFile);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                }
                catch (Exception ex)
                {
                    throw new InvalidArgumentsException($"Failed to create log directory '{logDir}': {ex.Message}");
                }
            }

            if (!Directory.Exists(source))
                throw new InvalidArgumentsException($"Source folder not found: {source}");
            if (!Directory.Exists(replica))
                Directory.CreateDirectory(replica);

            return new SyncConfig(source, replica, interval, logFile);
        }
    }
}
