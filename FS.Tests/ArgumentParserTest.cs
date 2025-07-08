using FolderSync.Exceptions;
using FolderSync.Utils;

namespace FS.Tests
{
    public class ArgumentParserTest
    {
        [Fact]
        public void Parse_ValidArguments_ReturnsConfig()
        {
            var temp = Path.GetTempPath();
            var src = Path.Combine(temp, "src");
            var rep = Path.Combine(temp, "rep");
            var log = Path.Combine(temp, "log.txt");
            var args = new[] { src, rep, "10", log };

            var config = ArgumentParser.Parse(args);

            Assert.NotNull(config);
            Assert.Equal(10, config.IntervalSeconds);
            Assert.Contains("src", config.SourcePath);
            Assert.Contains("rep", config.ReplicaPath);
            Assert.Contains("log.txt", config.LogFilePath);
        }

        [Fact]
        public void Parse_TooFewArguments_Throws()
        {
            var args = new[] { "a", "b", "1" };
            Assert.Throws<InvalidArgumentsException>(() => ArgumentParser.Parse(args));
        }

        [Fact]
        public void Parse_InvalidInterval_Throws()
        {
            var args = new[] { "a", "b", "zero", "c" };
            Assert.Throws<InvalidArgumentsException>(() => ArgumentParser.Parse(args));
        }
    }
}