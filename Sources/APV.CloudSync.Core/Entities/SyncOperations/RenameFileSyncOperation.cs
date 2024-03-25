using System;
using System.IO;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.SyncOperations
{
    public sealed class RenameFileSyncOperation : ExtendedSyncOperation
    {
        protected override void Invoke()
        {
            if (File.Exists(FullPath))
            {
                bool canRemove = (!File.Exists(ExtendedPath)) || (string.Compare(FullPath, ExtendedPath, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (canRemove)
                {
                    File.Move(FullPath, ExtendedPath);
                }
            }
        }

        public RenameFileSyncOperation(string fullPath, string newPath)
            : base(SyncOperationType.RenameFile, fullPath, newPath)
        {
        }
    }
}