using System;
using System.IO;
using FolderSync.Logging;
using FolderSync.Models;

namespace FolderSync.Services
{
    public class DirectoryProtectionService
    {
        private readonly SyncConfig _config;
        private readonly ILogger _logger;
        private readonly Dictionary<string, DateTime> _lastCheckTimes = new();

        public DirectoryProtectionService(SyncConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public void EnsureMainDirectoriesExist()
        {
            var currentTime = DateTime.Now;
            
            if (!Directory.Exists(_config.SourcePath))
            {
                try
                {
                    Directory.CreateDirectory(_config.SourcePath);
                    _logger.Log($"RECOVERY: Source directory was missing and has been recreated: {_config.SourcePath}");
                    
                    if (_lastCheckTimes.TryGetValue("source", out var lastSourceCheck))
                    {
                        var timeSinceLastRecreation = currentTime - lastSourceCheck;
                        if (timeSinceLastRecreation.TotalMinutes < 5)
                        {
                            _logger.Log($"WARNING: Source directory has been recreated multiple times in short period. This may indicate an external interference.");
                        }
                    }
                    _lastCheckTimes["source"] = currentTime;
                }
                catch (Exception ex)
                {
                    _logger.Log($"CRITICAL: Failed to recreate source directory '{_config.SourcePath}': {ex.Message}");
                    throw;
                }
            }

            if (!Directory.Exists(_config.ReplicaPath))
            {
                try
                {
                    Directory.CreateDirectory(_config.ReplicaPath);
                    _logger.Log($"RECOVERY: Replica directory was missing and has been recreated: {_config.ReplicaPath}");
                    
                    if (_lastCheckTimes.TryGetValue("replica", out var lastReplicaCheck))
                    {
                        var timeSinceLastRecreation = currentTime - lastReplicaCheck;
                        if (timeSinceLastRecreation.TotalMinutes < 5)
                        {
                            _logger.Log($"WARNING: Replica directory has been recreated multiple times in short period. This may indicate an external interference.");
                        }
                    }
                    _lastCheckTimes["replica"] = currentTime;
                }
                catch (Exception ex)
                {
                    _logger.Log($"CRITICAL: Failed to recreate replica directory '{_config.ReplicaPath}': {ex.Message}");
                    throw;
                }
            }
        }

        public bool IsProtectedDirectory(string directoryPath)
        {
            try
            {
                var fullPath = Path.GetFullPath(directoryPath);
                var sourceFullPath = Path.GetFullPath(_config.SourcePath);
                var replicaFullPath = Path.GetFullPath(_config.ReplicaPath);

                return string.Equals(fullPath, sourceFullPath, StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(fullPath, replicaFullPath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public void LogDirectoryProtection(string directoryPath, string operation)
        {
            _logger.Log($"PROTECTION: Prevented {operation} on protected directory: {directoryPath}");
        }
    }
}