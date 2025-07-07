using FolderSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSync.Services
{
    public class FolderSyncService
    {
        private readonly SyncConfig _config;


        public FolderSyncService(SyncConfig config)
        {
            _config = config;
        }
    }
}
