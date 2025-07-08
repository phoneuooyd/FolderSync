using System;
using System.IO;
using FolderSync.Commands;
using FolderSync.Logging;
using Xunit;

namespace FS.Tests
{
    public class DeleteCommandTests
    {
        private class TestLogger : ILogger
        {
            public string? LastMessage { get; private set; }
            public void Log(string message) => LastMessage = message;
            public void Dispose() { }
        }

        [Fact]
        public void Execute_DeletesFileAndLogs()
        {
            var temp = Path.GetTempPath();
            var file = Path.Combine(temp, "delete_me.txt");
            File.WriteAllText(file, "to delete");
            var logger = new TestLogger();

            var cmd = new DeleteFileCommand(file, logger);
            cmd.Execute();

            Assert.False(File.Exists(file));
            Assert.Contains("Deleted", logger.LastMessage);
            Assert.Contains(file, logger.LastMessage);
        }

        [Fact]
        public void Execute_DoesNothingIfFileDoesNotExist()
        {
            var temp = Path.GetTempPath();
            var file = Path.Combine(temp, "not_exists.txt");
            if (File.Exists(file)) File.Delete(file);
            var logger = new TestLogger();

            var cmd = new DeleteFileCommand(file, logger);
            cmd.Execute();

            Assert.Null(logger.LastMessage);
        }

        [Fact]
        public void Execute_DoesNotThrowOnMissingFile()
        {
            var temp = Path.GetTempPath();
            var file = Path.Combine(temp, "missing.txt");
            if (File.Exists(file)) File.Delete(file);
            var logger = new TestLogger();

            var cmd = new DeleteFileCommand(file, logger);
            var ex = Record.Exception(() => cmd.Execute());

            Assert.Null(ex);
        }
    }
}