using FolderSync.Exceptions;
using Xunit;

namespace FS.Tests.Exceptions.Tests
{
    public class SyncExceptionTests
    {
        [Fact]
        public void Constructor_SetsMessage()
        {
            var ex = new SyncException("msg", null);
            Assert.Equal("msg", ex.Message);
        }

        [Fact]
        public void Constructor_SetsInnerException()
        {
            var inner = new System.Exception("inner");
            var ex = new SyncException("msg", inner);
            Assert.Equal(inner, ex.InnerException);
        }

        [Fact]
        public void Exception_CanBeThrownAndCaught()
        {
            try
            {
                throw new SyncException("fail", null);
            }
            catch (SyncException ex)
            {
                Assert.Equal("fail", ex.Message);
            }
        }
    }
}