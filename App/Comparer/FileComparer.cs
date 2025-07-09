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
                return true; 

            if(CompareFileContentsByHash(sourceFile, replicaFile))
                return true;

            if (CompareFileContentsByByteComparison(sourceFile, replicaFile))
                return true;

            return false;
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

        private bool CompareFileContentsByByteComparison(string sourceFile, string replicaFile)
        {
            using var fs1 = File.OpenRead(sourceFile);
            using var fs2 = File.OpenRead(replicaFile);
            int b1, b2;
            do
            {
                b1 = fs1.ReadByte();
                b2 = fs2.ReadByte();
                if (b1 != b2) return true;
            } while (b1 != -1);
            return false; 
        }
    }
}