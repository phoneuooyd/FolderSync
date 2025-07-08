using System;
using System.IO;
using FolderSync.Models;
using FolderSync.Logging;
using FolderSync.Services;
using Xunit;

namespace FS.Tests.Services.Tests
{
    public class DirectoryProtectionServiceTests
    {
        private class TestLogger : ILogger
        {
            public string? LastMessage { get; private set; }
            public void Log(string message) => LastMessage = message;
            public void Dispose() { }
        }

        [Fact]
        public void EnsureMainDirectoriesExist_CreatesMissingDirs()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var service = new DirectoryProtectionService(config, logger);

            try
            {
                service.EnsureMainDirectoriesExist();
                Assert.True(Directory.Exists(src));
                Assert.True(Directory.Exists(rep));
            }
            finally
            {
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
            }
        }

        [Fact]
        public void IsProtectedDirectory_ReturnsTrueForMainDirs()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(rep);
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var service = new DirectoryProtectionService(config, logger);

            try
            {
                Assert.True(service.IsProtectedDirectory(src));
                Assert.True(service.IsProtectedDirectory(rep));
            }
            finally
            {
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
            }
        }

        [Fact]
        public void IsProtectedDirectory_ReturnsFalseForOtherDirs()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            var other = Path.Combine(temp, "other");
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(rep);
            Directory.CreateDirectory(other);
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var service = new DirectoryProtectionService(config, logger);

            try
            {
                Assert.False(service.IsProtectedDirectory(other));
            }
            finally
            {
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
            }
        }

        [Fact]
        public void LogDirectoryProtection_LogsMessage()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(rep);
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var service = new DirectoryProtectionService(config, logger);

            service.LogDirectoryProtection(src, "deletion");
            Assert.Contains("PROTECTION", logger.LastMessage);
            Assert.Contains("deletion", logger.LastMessage);
        }
    }
}