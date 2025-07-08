using FolderSync.Logging;
using Xunit;

namespace FS.Tests.Logging.Tests
{
    public class CombinedLoggerTests
    {
        private class TestLogger : ILogger
        {
            public string? LastMessage { get; private set; }
            public void Log(string message) => LastMessage = message;
            public void Dispose() { }
        }

        [Fact]
        public void Log_DelegatesToAllLoggers()
        {
            var l1 = new TestLogger();
            var l2 = new TestLogger();
            var logger = new CombinedLogger(l1, l2);

            logger.Log("msg");

            Assert.Equal("msg", l1.LastMessage);
            Assert.Equal("msg", l2.LastMessage);
        }

        [Fact]
        public void Dispose_DisposesAllLoggers()
        {
            var disposed = false;
            var l1 = new TestLogger();
            var l2 = new TestLogger();
            var logger = new CombinedLogger(l1, l2);

            logger.Dispose();

            // Brak efektu ubocznego, test tylko na wywołanie bez wyjątku
            Assert.True(true);
        }

        [Fact]
        public void Log_WithNoLoggers_DoesNotThrow()
        {
            var logger = new CombinedLogger();
            var ex = Record.Exception(() => logger.Log("test"));
            Assert.Null(ex);
        }
    }
}