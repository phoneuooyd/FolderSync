using System;
using System.IO;
using FolderSync.Logging;
using Xunit;

namespace FS.Tests.Logging.Tests
{
    public class FileLoggerTests
    {
        [Fact]
        public void Log_WritesToFile()
        {
            var temp = Path.GetTempFileName();
            try
            {
                using var logger = new FileLogger(temp);
                logger.Log("test123");
                logger.Dispose();

                var content = File.ReadAllText(temp);
                Assert.Contains("test123", content);
            }
            finally
            {
                File.Delete(temp);
            }
        }

        [Fact]
        public void Dispose_ClosesFile()
        {
            var temp = Path.GetTempFileName();
            var logger = new FileLogger(temp);
            logger.Dispose();

            // Plik powinien być dostępny do zapisu po Dispose
            File.AppendAllText(temp, "ok");
            File.Delete(temp);
        }

        [Fact]
        public void Log_MultipleLines_Appends()
        {
            var temp = Path.GetTempFileName();
            try
            {
                using var logger = new FileLogger(temp);
                logger.Log("line1");
                logger.Log("line2");
                logger.Dispose();

                var content = File.ReadAllText(temp);
                Assert.Contains("line1", content);
                Assert.Contains("line2", content);
            }
            finally
            {
                File.Delete(temp);
            }
        }
    }
}