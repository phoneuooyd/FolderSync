using System;
using System.IO;
using FolderSync.Logging;
using FolderSync.Models;

namespace FolderSync.Services
{
    public class FileSystemMonitor
    {
        private readonly SyncConfig _config;
        private readonly ILogger _logger;
        private readonly Dictionary<string, long> _lastFileCounts = new();
        private readonly Dictionary<string, DateTime> _lastHealthChecks = new();

        public FileSystemMonitor(SyncConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public void PerformHealthCheck()
        {
            var currentTime = DateTime.Now;
            
            // Sprawdzenie czy g³ówne katalogi istniej¹
            CheckDirectoryHealth(_config.SourcePath, "source");
            CheckDirectoryHealth(_config.ReplicaPath, "replica");

            // Sprawdzenie czy nie ma podejrzanej aktywnoœci (np. drastyczne zmiany w liczbie plików)
            CheckForSuspiciousActivity();

            _lastHealthChecks["last_check"] = currentTime;
        }

        private void CheckDirectoryHealth(string directoryPath, string directoryType)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    _logger.Log($"HEALTH CHECK: {directoryType} directory is missing: {directoryPath}");
                    return;
                }

                var fileCount = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Length;
                var directoryCount = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories).Length;

                _logger.Log($"HEALTH CHECK: {directoryType} directory - Files: {fileCount}, Subdirectories: {directoryCount}");

                // Sprawdzenie czy nie ma drastycznych zmian w liczbie plików
                if (_lastFileCounts.TryGetValue(directoryType, out var lastCount))
                {
                    var changePercentage = Math.Abs((double)(fileCount - lastCount) / Math.Max(lastCount, 1)) * 100;
                    if (changePercentage > 50 && lastCount > 10) // Ponad 50% zmian i wiêcej ni¿ 10 plików wczeœniej
                    {
                        _logger.Log($"WARNING: Significant change in {directoryType} directory file count: {lastCount} -> {fileCount} ({changePercentage:F1}% change)");
                    }
                }

                _lastFileCounts[directoryType] = fileCount;
            }
            catch (Exception ex)
            {
                _logger.Log($"HEALTH CHECK ERROR: Failed to check {directoryType} directory health: {ex.Message}");
            }
        }

        private void CheckForSuspiciousActivity()
        {
            try
            {
                // Sprawdzenie czy katalogi source i replica nie s¹ przypadkiem tymi samymi
                if (Directory.Exists(_config.SourcePath) && Directory.Exists(_config.ReplicaPath))
                {
                    var sourceFullPath = Path.GetFullPath(_config.SourcePath);
                    var replicaFullPath = Path.GetFullPath(_config.ReplicaPath);
                    
                    if (string.Equals(sourceFullPath, replicaFullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.Log("CRITICAL: Source and replica directories are the same! This will cause data loss!");
                    }
                }

                // Sprawdzenie czy katalogi nie zosta³y zamienione miejscami
                if (_lastFileCounts.TryGetValue("source", out var sourceCount) && 
                    _lastFileCounts.TryGetValue("replica", out var replicaCount))
                {
                    if (sourceCount == 0 && replicaCount > 0)
                    {
                        _logger.Log("WARNING: Source directory is empty while replica has files. This may indicate a configuration error.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"SUSPICIOUS ACTIVITY CHECK ERROR: {ex.Message}");
            }
        }

        public bool ShouldPerformHealthCheck()
        {
            if (!_lastHealthChecks.TryGetValue("last_check", out var lastCheck))
                return true;

            // Wykonuj sprawdzenie co 5 minut
            return DateTime.Now - lastCheck > TimeSpan.FromMinutes(5);
        }
    }
}