using System;
using FolderSync.Utils;

namespace FolderSync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var config = ArgumentParser.Parse(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
            }
        }
    }
}
