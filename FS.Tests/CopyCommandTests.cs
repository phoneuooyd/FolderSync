using System;
using System.IO;
using FolderSync.Commands;
using FolderSync.Logging;
using Xunit;

namespace FS.Tests
{
    public class CopyCommandTests
    {
        private class TestLogger : ILogger
        {
            public string? LastMessage { get; private set; }
            public void Log(string message) => LastMessage = message;
            public void Dispose() { }
        }

        [Fact]
        public void Execute_CopiesFileAndLogs()
        {
            var temp = Path.GetTempPath();
            var src = Path.Combine(temp, "copy_src.txt");
            var dst = Path.Combine(temp, "copy_dst.txt");
            File.WriteAllText(src, "abc");
            if (File.Exists(dst)) File.Delete(dst);
            var logger = new TestLogger();

            try
            {
                var cmd = new CopyFileCommand(src, dst, logger);
                cmd.Execute();

                Assert.True(File.Exists(dst));
                Assert.Equal("abc", File.ReadAllText(dst));
                Assert.Contains("Copied", logger.LastMessage);
                Assert.Contains(src, logger.LastMessage);
                Assert.Contains(dst, logger.LastMessage);
            }
            finally
            {
                if (File.Exists(src)) File.Delete(src);
                if (File.Exists(dst)) File.Delete(dst);
            }
        }

        [Fact]
        public void Execute_CreatesDestinationDirectoryIfNotExists()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var src = Path.Combine(temp, "src.txt");
            var dstDir = Path.Combine(temp, "subdir");
            var dst = Path.Combine(dstDir, "dst.txt");
            Directory.CreateDirectory(temp);
            File.WriteAllText(src, "xyz");
            var logger = new TestLogger();

            try
            {
                var cmd = new CopyFileCommand(src, dst, logger);
                cmd.Execute();

                Assert.True(File.Exists(dst));
                Assert.Equal("xyz", File.ReadAllText(dst));
            }
            finally
            {
                if (Directory.Exists(temp)) Directory.Delete(temp, true);
            }
        }

        [Fact]
        public void Execute_OverwritesExistingFile()
        {
            var temp = Path.GetTempPath();
            var src = Path.Combine(temp, "src.txt");
            var dst = Path.Combine(temp, "dst.txt");
            File.WriteAllText(src, "new");
            File.WriteAllText(dst, "old");
            var logger = new TestLogger();

            try
            {
                var cmd = new CopyFileCommand(src, dst, logger);
                cmd.Execute();

                Assert.Equal("new", File.ReadAllText(dst));
            }
            finally
            {
                if (File.Exists(src)) File.Delete(src);
                if (File.Exists(dst)) File.Delete(dst);
            }
        }
    }
}
