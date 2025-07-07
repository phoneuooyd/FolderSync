using FolderSync.Commands;
using FolderSync.Comparers;
using FolderSync.Logging;
using FolderSync.Models;
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

        public FolderSyncService(SyncConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Run()
        {
            EnsureDirectoriesExist();

            while (true)
            {
                try
                {
                    Synchronize();
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error during synchronization: {ex.Message}");
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
            SyncDirectory(_config.SourcePath, _config.ReplicaPath);
            RemoveExtraFiles(_config.SourcePath, _config.ReplicaPath);
        }

        private void SyncDirectory(string sourceDir, string replicaDir)
        {
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
                if (!Directory.Exists(sourceSubDir))
                {
                    Directory.Delete(dir, true);
                    _logger.Log($"Deleted directory {dir}");
                }
                else
                {
                    RemoveExtraFiles(sourceSubDir, dir);
                }
            }
        }
    }
}

