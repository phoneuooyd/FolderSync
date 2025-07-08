using System;
using System.IO;
using FolderSync.Models;
using FolderSync.Exceptions;

namespace FolderSync.Utils
{
    public static class ArgumentParser
    {
        public static SyncConfig Parse(string[] args)
        {
            if (args.Length != 4)
                throw new InvalidArgumentsException("Expected arguments: <sourcePath> <replicaPath> <intervalSeconds> <logFilePath>");

            var source = ValidateAndNormalizePath(args[0], "source");
            var replica = ValidateAndNormalizePath(args[1], "replica");
            
            if (!int.TryParse(args[2], out var interval) || interval <= 0)
                throw new InvalidArgumentsException("Interval must be a positive integer");
            
            var logFile = ValidateAndNormalizePath(args[3], "log file");

            if (IsSameDirectory(source, replica))
                throw new InvalidArgumentsException("Source and replica paths cannot be the same directory");

            if (Directory.Exists(logFile))
                throw new InvalidArgumentsException($"Log file path points to a directory: {logFile}");

            var logDir = Path.GetDirectoryName(logFile);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                }
                catch (Exception ex)
                {
                    throw new InvalidArgumentsException($"Failed to create log directory '{logDir}': {ex.Message}");
                }
            }

            if (!Directory.Exists(source))
            {
                try
                {
                    Directory.CreateDirectory(source);
                }
                catch (Exception ex)
                {
                    throw new InvalidArgumentsException($"Failed to create source directory '{source}': {ex.Message}");
                }
            }

            if (!Directory.Exists(replica))
            {
                try
                {
                    Directory.CreateDirectory(replica);
                }
                catch (Exception ex)
                {
                    throw new InvalidArgumentsException($"Failed to create replica directory '{replica}': {ex.Message}");
                }
            }

            return new SyncConfig(source, replica, interval, logFile);
        }

        private static string ValidateAndNormalizePath(string path, string pathType)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new InvalidArgumentsException($"The {pathType} path cannot be empty");

            path = path.Trim();
            
            if (IsInvalidPath(path))
                throw new InvalidArgumentsException($"Invalid {pathType} path: '{path}'");

            if (HasTypicalPathErrors(path))
                throw new InvalidArgumentsException($"The {pathType} path appears to have formatting errors: '{path}'. Check for missing backslashes or colons.");

            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception ex)
            {
                throw new InvalidArgumentsException($"Invalid {pathType} path format: '{path}'. {ex.Message}");
            }
        }

        private static bool IsInvalidPath(string path)
        {
            if (path.Length == 0) return true;
            
            char[] invalidChars = Path.GetInvalidPathChars();
            foreach (char c in invalidChars)
            {
                if (path.Contains(c))
                    return true;
            }

            if (path.Contains("..\\..") || path.Contains("../.."))
                return true;

            return false;
        }

        private static bool HasTypicalPathErrors(string path)
        {
            if (path.Length < 3) return false;

            if (char.IsLetter(path[0]) && path[1] == ':' && path[2] != '\\' && path[2] != '/')
                return true;

            if (path.Contains(":\\") == false && path.Contains(":/") == false && 
                path.Length > 2 && char.IsLetter(path[0]) && path[1] == ':')
                return true;

            if (path.Contains("\\\\") || path.Contains("//"))
                return true;

            return false;
        }

        private static bool IsSameDirectory(string path1, string path2)
        {
            try
            {
                var fullPath1 = Path.GetFullPath(path1);
                var fullPath2 = Path.GetFullPath(path2);
                return string.Equals(fullPath1, fullPath2, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
