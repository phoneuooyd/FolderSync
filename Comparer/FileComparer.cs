using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace FolderSync.Comparers
{
    public class FileComparer : IFileComparer
    {
        public bool FilesAreDifferent(string sourceFile, string replicaFile)
        {
            var sourceInfo = new FileInfo(sourceFile);
            var replicaInfo = new FileInfo(replicaFile);

            if (sourceInfo.Length != replicaInfo.Length)
            {
                return true; 
            }

            if (sourceInfo.LastWriteTimeUtc != replicaInfo.LastWriteTimeUtc)
            {
                return true;
            }

            return CompareFileContentsByHash(sourceFile, replicaFile);
        }

        private bool CompareFileContentsByHash(string sourceFile, string replicaFile)
        {
            using var md5 = MD5.Create();

            using var sourceStream = File.OpenRead(sourceFile);
            using var replicaStream = File.OpenRead(replicaFile);

            var sourceHash = md5.ComputeHash(sourceStream);
            var replicaHash = md5.ComputeHash(replicaStream);

            return !sourceHash.SequenceEqual(replicaHash);
        }
    }
}