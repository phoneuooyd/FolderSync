using System;
using System.IO;
using FolderSync.Models;
using FolderSync.Logging;
using FolderSync.Services;
using Xunit;

namespace FS.Tests.Services.Tests
{
    public class FileSystemMonitorTests
    {
        private class TestLogger : ILogger
        {
            public string? LastMessage { get; private set; }
            public void Log(string message) => LastMessage = message;
            public void Dispose() { }
        }

        [Fact]
        public void PerformHealthCheck_LogsForMissingDirs()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var monitor = new FileSystemMonitor(config, logger);

            monitor.PerformHealthCheck();

            Assert.Contains("missing", logger.LastMessage);
        }

        [Fact]
        public void ShouldPerformHealthCheck_TrueIfNeverChecked()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var monitor = new FileSystemMonitor(config, logger);

            Assert.True(monitor.ShouldPerformHealthCheck());
        }

        [Fact]
        public void ShouldPerformHealthCheck_FalseIfCheckedRecently()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var monitor = new FileSystemMonitor(config, logger);

            monitor.PerformHealthCheck();
            Assert.False(monitor.ShouldPerformHealthCheck());
        }

        [Fact]
        public void PerformHealthCheck_LogsHealthCheck()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temp);
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(rep);
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var monitor = new FileSystemMonitor(config, logger);

            monitor.PerformHealthCheck();

            Assert.Contains("HEALTH CHECK", logger.LastMessage);
            if (Directory.Exists(temp)) Directory.Delete(temp, true);
        }
    }
}