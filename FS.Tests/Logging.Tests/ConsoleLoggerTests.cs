using FolderSync.Logging;
using Xunit;

namespace FS.Tests.Logging.Tests
{
    public class ConsoleLoggerTest
    {
        [Fact]
        public void Log_DoesNotThrow_WhenCalledWithMessage()
        {
            var logger = new ConsoleLogger();
            var exception = Record.Exception(() => logger.Log("Test message"));
            Assert.Null(exception);
        }

        [Fact]
        public void Log_DoesNotThrow_WhenCalledWithEmptyString()
        {
            var logger = new ConsoleLogger();
            var exception = Record.Exception(() => logger.Log(string.Empty));
            Assert.Null(exception);
        }

        [Fact]
        public void Dispose_DoesNotThrow()
        {
            var logger = new ConsoleLogger();
            var exception = Record.Exception(() => logger.Dispose());
            Assert.Null(exception);
        }
    }
}