using System;
using System.IO;
using FolderSync.Comparers;
using Xunit;

namespace FS.Tests.Comparer.Tests
{
    public class FileComparerTests
    {
        [Fact]
        public void FilesAreDifferent_IdenticalFiles_ReturnsFalse()
        {
            var temp = Path.GetTempPath();
            var file1 = Path.Combine(temp, "fc1.txt");
            var file2 = Path.Combine(temp, "fc2.txt");
            File.WriteAllText(file1, "abc");
            File.WriteAllText(file2, "abc");
            var comparer = new FileComparer();

            try
            {
                Assert.False(comparer.FilesAreDifferent(file1, file2));
            }
            finally
            {
                File.Delete(file1);
                File.Delete(file2);
            }
        }

        [Fact]
        public void FilesAreDifferent_DifferentContent_ReturnsTrue()
        {
            var temp = Path.GetTempPath();
            var file1 = Path.Combine(temp, "fc1.txt");
            var file2 = Path.Combine(temp, "fc2.txt");
            File.WriteAllText(file1, "abc");
            File.WriteAllText(file2, "xyz");
            var comparer = new FileComparer();

            try
            {
                Assert.True(comparer.FilesAreDifferent(file1, file2));
            }
            finally
            {
                File.Delete(file1);
                File.Delete(file2);
            }
        }

        [Fact]
        public void FilesAreDifferent_DifferentSize_ReturnsTrue()
        {
            var temp = Path.GetTempPath();
            var file1 = Path.Combine(temp, "fc1.txt");
            var file2 = Path.Combine(temp, "fc2.txt");
            File.WriteAllText(file1, "a");
            File.WriteAllText(file2, "ab");
            var comparer = new FileComparer();

            try
            {
                Assert.True(comparer.FilesAreDifferent(file1, file2));
            }
            finally
            {
                File.Delete(file1);
                File.Delete(file2);
            }
        }
    }
}