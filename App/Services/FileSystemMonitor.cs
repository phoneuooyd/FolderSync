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
            
            CheckDirectoryHealth(_config.SourcePath, "source");
            CheckDirectoryHealth(_config.ReplicaPath, "replica");

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

                if (_lastFileCounts.TryGetValue(directoryType, out var lastCount))
                {
                    var changePercentage = Math.Abs((double)(fileCount - lastCount) / Math.Max(lastCount, 1)) * 100;
                    if (changePercentage > 50 && lastCount > 10)
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
                if (Directory.Exists(_config.SourcePath) && Directory.Exists(_config.ReplicaPath))
                {
                    var sourceFullPath = Path.GetFullPath(_config.SourcePath);
                    var replicaFullPath = Path.GetFullPath(_config.ReplicaPath);
                    
                    if (string.Equals(sourceFullPath, replicaFullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.Log("CRITICAL: Source and replica directories are the same! This will cause data loss!");
                    }
                }

                if (_lastFileCounts.TryGetValue("source", out var sourceCount) && 
                    _lastFileCounts.TryGetValue("replica", out var replicaCount))
                {
                    if (sourceCount == 0 && replicaCount > 0)
                    {
                        _logger.Log("WARNING: Source directory is empty while replica has files. This may indicate unexpected intervention.");
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

            return DateTime.Now - lastCheck > TimeSpan.FromMinutes(5);
        }
    }
}