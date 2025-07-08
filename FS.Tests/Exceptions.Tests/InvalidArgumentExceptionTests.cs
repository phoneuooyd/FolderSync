using FolderSync.Exceptions;
using Xunit;

namespace FS.Tests.Exceptions.Tests
{
    public class InvalidArgumentExceptionTests
    {
        [Fact]
        public void Constructor_SetsMessage()
        {
            var ex = new InvalidArgumentsException("msg");
            Assert.Equal("msg", ex.Message);
        }

        [Fact]
        public void Exception_IsExceptionType()
        {
            var ex = new InvalidArgumentsException("err");
            Assert.IsAssignableFrom<System.Exception>(ex);
        }

        [Fact]
        public void Exception_CanBeThrownAndCaught()
        {
            try
            {
                throw new InvalidArgumentsException("fail");
            }
            catch (InvalidArgumentsException ex)
            {
                Assert.Equal("fail", ex.Message);
            }
        }
    }
}