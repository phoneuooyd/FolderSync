using System;
using System.IO;
using FolderSync.Models;
using FolderSync.Logging;
using FolderSync.App;
using Xunit;

namespace FS.Tests.App.Tests
{
    public class FolderSyncServiceTests
    {
        private class TestLogger : ILogger
        {
            public string? LastMessage { get; private set; }
            public void Log(string message) => LastMessage = message;
            public void Dispose() { }
        }

        [Fact]
        public void EnsureDirectoriesExist_CreatesMissingDirs()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var service = new FolderSyncService(config, logger);

            try
            {
                var method = typeof(FolderSyncService).GetMethod("EnsureDirectoriesExist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(service, null);

                Assert.True(Directory.Exists(src));
                Assert.True(Directory.Exists(rep));
            }
            finally
            {
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
            }
        }

        [Fact]
        public void SyncDirectory_CreatesReplicaFiles()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(rep);
            File.WriteAllText(Path.Combine(src, "a.txt"), "abc");
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var service = new FolderSyncService(config, logger);

            try
            {
                var method = typeof(FolderSyncService).GetMethod("SyncDirectory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(service, new object[] { src, rep });

                Assert.True(File.Exists(Path.Combine(rep, "a.txt")));
            }
            finally
            {
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
            }
        }

        [Fact]
        public void RemoveExtraFiles_DeletesUnmatchedFiles()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(rep);
            File.WriteAllText(Path.Combine(rep, "b.txt"), "to delete");
            var config = new SyncConfig(src, rep, 1, Path.Combine(temp, "log.txt"));
            var logger = new TestLogger();
            var service = new FolderSyncService(config, logger);

            try
            {
                var method = typeof(FolderSyncService).GetMethod("RemoveExtraFiles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(service, new object[] { src, rep });

                Assert.False(File.Exists(Path.Combine(rep, "b.txt")));
            }
            finally
            {
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
            }
        }
    }
}