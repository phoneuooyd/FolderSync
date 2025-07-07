using System;
using FolderSync.Utils;
using FolderSync.Logging;
using FolderSync.App;

namespace FolderSync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var config = ArgumentParser.Parse(args);
                
                // Tworzenie loggerów
                var fileLogger = new FileLogger(config.LogFilePath);
                var consoleLogger = new ConsoleLogger();
                var combinedLogger = new CombinedLogger(fileLogger, consoleLogger);

                // Uruchomienie serwisu synchronizacji
                var syncService = new FolderSyncService(config, combinedLogger);
                
                Console.WriteLine($"Starting folder synchronization...");
                Console.WriteLine($"Source: {config.SourcePath}");
                Console.WriteLine($"Replica: {config.ReplicaPath}");
                Console.WriteLine($"Interval: {config.IntervalSeconds} seconds");
                Console.WriteLine($"Log file: {config.LogFilePath}");
                Console.WriteLine("Press Ctrl+C to stop...");
                
                syncService.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Launch the app from the console using dotnet run");
                Console.WriteLine($"Fatal error: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}
