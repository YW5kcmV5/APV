using System;
using System.IO;
using APV.CloudSync.Common;
using APV.Common;

namespace APV.CloudSync.Core.Entities.SyncOperations
{
    public sealed class CopyFileSyncOperation : ExtendedSyncOperation
    {
        private string _checksum;

        protected override bool IsReady()
        {
            try
            {
                string checksum = IOUtility.GetChecksum(FullPath);
                if (checksum != _checksum)
                {
                    _checksum = checksum;
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override void Invoke()
        {
            if (File.Exists(FullPath))
            {
                string directory = Path.GetDirectoryName(ExtendedPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.Copy(FullPath, ExtendedPath, true);
            }
        }

        public CopyFileSyncOperation(string fullPath, string newPath)
            : base(SyncOperationType.CopyFile, fullPath, newPath)
        {
        }
    }
}