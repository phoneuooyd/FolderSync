using FolderSync.Commands;
using FolderSync.Comparers;
using FolderSync.Logging;
using FolderSync.Models;
using FolderSync.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.App
{
    public class FolderSyncService
    {
        private readonly SyncConfig _config;
        private readonly ILogger _logger;
        private readonly IFileComparer _comparer = new FileComparer();
        private readonly DirectoryProtectionService _protectionService;
        private readonly FileSystemMonitor _fileSystemMonitor;

        public FolderSyncService(SyncConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
            _protectionService = new DirectoryProtectionService(config, logger);
            _fileSystemMonitor = new FileSystemMonitor(config, logger);
        }

        public void Run()
        {
            EnsureDirectoriesExist();
            _logger.Log("FolderSync service started with enhanced directory protection");

            while (true)
            {
                try
                {
                    _protectionService.EnsureMainDirectoriesExist();

                    if (_fileSystemMonitor.ShouldPerformHealthCheck())
                    {
                        _fileSystemMonitor.PerformHealthCheck();
                    }

                    Synchronize();
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error during synchronization: {ex.Message}");

                    if (ex.Message.Contains("Could not find a part of the path") ||
                        ex.Message.Contains("DirectoryNotFoundException") ||
                        ex.Message.Contains("path") && ex.Message.Contains("not"))
                    {
                        _logger.Log("Attempting to recover from missing directory error...");
                        try
                        {
                            _protectionService.EnsureMainDirectoriesExist();
                            _logger.Log("Directory recovery completed successfully");
                        }
                        catch (Exception recoveryEx)
                        {
                            _logger.Log($"Failed to recover directories: {recoveryEx.Message}");
                        }
                    }
                }

                Thread.Sleep(_config.IntervalSeconds * 1000);
            }
        }

        private void EnsureDirectoriesExist()
        {
            try
            {
                if (!Directory.Exists(_config.SourcePath))
                {
                    Directory.CreateDirectory(_config.SourcePath);
                    _logger.Log($"Created source directory: {_config.SourcePath}");
                }

                if (!Directory.Exists(_config.ReplicaPath))
                {
                    Directory.CreateDirectory(_config.ReplicaPath);
                    _logger.Log($"Created replica directory: {_config.ReplicaPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to ensure directories exist: {ex.Message}");
                throw;
            }
        }

        private void Synchronize()
        {
            try
            {
                SyncDirectory(_config.SourcePath, _config.ReplicaPath);
                RemoveExtraFiles(_config.SourcePath, _config.ReplicaPath);
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Log($"Directory not found during synchronization: {ex.Message}");
                _protectionService.EnsureMainDirectoriesExist();
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Log($"Access denied during synchronization: {ex.Message}");
                throw;
            }
        }

        private void SyncDirectory(string sourceDir, string replicaDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                _logger.Log($"Source directory missing during sync: {sourceDir}");
                _protectionService.EnsureMainDirectoriesExist();
                return;
            }

            if (!Directory.Exists(replicaDir))
            {
                _logger.Log($"Replica directory missing during sync: {replicaDir}");
                Directory.CreateDirectory(replicaDir);
            }

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var replicaFile = Path.Combine(replicaDir, fileName);

                if (!File.Exists(replicaFile) || _comparer.FilesAreDifferent(file, replicaFile))
                {
                    new CopyFileCommand(file, replicaFile, _logger).Execute();
                }
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(dir);
                var replicaSubDir = Path.Combine(replicaDir, dirName);
                if (!Directory.Exists(replicaSubDir))
                    Directory.CreateDirectory(replicaSubDir);
                SyncDirectory(dir, replicaSubDir);
            }
        }

        private void RemoveExtraFiles(string sourceDir, string replicaDir)
        {
            if (!Directory.Exists(replicaDir))
            {
                _logger.Log($"Replica directory missing during cleanup: {replicaDir}");
                return;
            }

            foreach (var file in Directory.GetFiles(replicaDir))
            {
                var fileName = Path.GetFileName(file);
                var sourceFile = Path.Combine(sourceDir, fileName);
                if (!File.Exists(sourceFile))
                {
                    new DeleteFileCommand(file, _logger).Execute();
                }
            }

            foreach (var dir in Directory.GetDirectories(replicaDir))
            {
                var dirName = Path.GetFileName(dir);
                var sourceSubDir = Path.Combine(sourceDir, dirName);

                if (_protectionService.IsProtectedDirectory(dir))
                {
                    _protectionService.LogDirectoryProtection(dir, "deletion");
                    continue;
                }

                if (!Directory.Exists(sourceSubDir))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        _logger.Log($"Deleted directory {dir}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"Failed to delete directory {dir}: {ex.Message}");
                    }
                }
                else
                {
                    RemoveExtraFiles(sourceSubDir, dir);
                }
            }
        }
    }
}

